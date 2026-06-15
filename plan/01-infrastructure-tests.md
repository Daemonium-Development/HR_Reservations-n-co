# 01 — Stabilize the infrastructure integration tests

## Goal

Make the `DD.Infra.Test` integration-test suite pass reliably. It currently fails **7 of
40** tests and reports `System.IO.IOException` cleanup failures on every class. All fixes
live in the test project; no changes to `DebugDiner.Infrastructure`, `DebugDiner.Domain`,
or the `Migrations/*.sql` files.

## Context

- Source request: `requests/2026-06-infrastructure-test.md`. Key constraint: *"adjusting
  the application to fit tests is not done today. How things stand at this point is how
  things work."* → fix **tests and the test fixture only**, never the app.
- Decisions for this changeset are recorded in `plan/00-decisions.md` under
  `2026-06-15 — Stabilize infrastructure tests`. Read them; they explain every "why" below.
- Baseline architecture is in `plan/STATUS.md`. Relevant facts:
  - Tests are real integration tests: each test class gets its own throwaway on-disk SQLite
    DB via `DatabaseFixture` (`IClassFixture`), and tests within a class are ordered 1..8 by
    the `TestPriority` orderer. The CRUD tests are **interdependent and ordered** (Get → …
    → Create id=2 → Update id=2 → Delete) and share one DB per class. Preserve that shape.
  - The app does **not** seed the `user` table in migrations. Instead `Program.Main` seeds
    an admin at runtime via `AuthService.RegisterAsync`. The `DataService`-only fixture
    bypasses `Program.Main`, so no user exists in the test DB — this is the root cause of
    most failures.

### Current failures (verified by running `dotnet test DD.Infra.Test/DD.Infra.Test.csproj`)

| Test | Symptom | Root cause |
|---|---|---|
| `UserRepositoryTests.GetSingleUser_ShouldSucceed` | found 0, expected 1 | no `user` id=1 seed |
| `UserRepositoryTests.GetMultipleUsers_ShouldGiveOnlyOne` | found 0 | no `user` id=1 seed |
| `UserRepositoryTests.GetAllUsers_ShouldSucceed` | found 0 | no `user` id=1 seed |
| `UserRepositoryTests.UpdateUser_ShouldSucceed` | found 0 | autoincrement shift: `Create` made id=1 (not 2), so `Update(id=2)` hits no row |
| `ReservationRepositoryTests.CreateReservation_ShouldSucceed` | found 0 | FK `reservation.user_id → user(id=1)` violated (FK ON for new inserts); exception swallowed |
| `ReservationRepositoryTests.UpdateReservation_ShouldSucceed` | found 0 | same FK violation as Create |
| `TableRepositoryTests.DeleteTable_ShouldReduceCount` | found 1, expected 0 | `TableRepository.Delete` does not cascade; seeded reservation's FK to `table(id=1)` blocks deleting table 1 |
| *(all 5 classes)* | `Test Class Cleanup Failure … System.IO.IOException` | SQLite connection pooling keeps the file locked when `Dispose` calls `File.Delete` |

Seeding a `user` id=1 fixes the first six rows (User Get/Update + Reservation Create/Update).
The Table delete and the IOException cleanup are separate fixes.

## Implementation plan

All paths are under `DD.Infra.Test/`.

### 1. `DD.Infra.Test.csproj` — package references

- **Add** an explicit reference so the cleanup code can call `SqliteConnection`:
  ```xml
  <PackageReference Include="Microsoft.Data.Sqlite" Version="10.0.5" />
  ```
  (Match the version used by `DebugDiner.Infrastructure.csproj`; it is otherwise only
  transitive.)
- **Remove** the unused `Microsoft.Extensions.Logging` reference (the fixture mocks
  Serilog's `ILogger`, not the MS abstraction).
- Leave `BCrypt.Net-Next`, `FluentAssertions`, `Moq`, `xunit*`, `coverlet.collector`,
  `Microsoft.NET.Test.Sdk` as-is.

### 2. `DatabaseFixture.cs` — seed a user + robust cleanup

- Add `using Microsoft.Data.Sqlite;`.
- **After** `_db.StartAsync().Wait();` in the constructor, insert a single user with
  `id=1` over the live connection, mirroring the migration seed style. The column set must
  match `user` (`name, email, password_hash, role`) and `role` must be one of the CHECK
  values (`Admin`/`Employee`/`Customer`/`Manager`). The `password_hash` only needs to be
  NOT NULL — a fixed string is fine (no test asserts it). Example shape:
  ```csharp
  var seed = _db.Connection!.CreateCommand();
  seed.CommandText =
      "INSERT INTO `user` (`name`, `email`, `password_hash`, `role`, `created_at`, `updated_at`) " +
      "VALUES ('Seed User', 'seed@example.com', 'seed-hash', 'Admin', DATETIME('now'), DATETIME('now'));";
  seed.ExecuteNonQuery();
  ```
  This inserts into an empty `user` table → `id=1` (AUTOINCREMENT), satisfying the seeded
  reservation's FK and the User Get tests. It runs with `foreign_keys=ON` (no FK deps on
  `user`), so it is safe.
- **Rewrite `Dispose`** to release the file lock and clean up WAL sidecars:
  ```csharp
  public void Dispose()
  {
      _db?.Dispose();
      SqliteConnection.ClearAllPools();
      GC.Collect();
      GC.WaitForPendingFinalizers();
      TryDelete(_dbPath);
      TryDelete(_dbPath + "-wal");
      TryDelete(_dbPath + "-shm");
      GC.SuppressFinalize(this);
  }

  private static void TryDelete(string path)
  {
      for (var attempt = 0; attempt < 5; attempt++)
      {
          try
          {
              if (File.Exists(path)) File.Delete(path);
              return;
          }
          catch (IOException)
          {
              Thread.Sleep(50);
          }
      }
  }
  ```
- Keep the repository getter methods unchanged.

### 3. Add the classes to the `Database` collection (serialize)

Add `[Collection("Database")]` to each of the five test classes so they run sequentially
(required because `ClearAllPools()` is process-global). This activates the existing
`DatabaseTestCollection` / `[CollectionDefinition("Database")]`. The `[TestCaseOrderer(...)]`
attribute and `IClassFixture<DatabaseFixture>` stay; a class can carry both a collection and
a class fixture.

- `ArrangementRepositoryTests.cs`
- `MenuRepositoryTests.cs`
- `ReservationRepositoryTests.cs`
- `TableRepositoryTests.cs`
- `UserRepositoryTests.cs`

Example (top of each class):
```csharp
[Collection("Database")]
[TestCaseOrderer("DD.Infra.Test.PriorityOrderer", "DD.Infra.Test")]
public class UserRepositoryTests(...) : IClassFixture<DatabaseFixture>
```

> Note: `DatabaseTestCollection` only carries `[CollectionDefinition("Database")]`; it does
> **not** declare an `ICollectionFixture<>`, so each class keeps its own per-class
> `DatabaseFixture` instance/DB. The collection is used purely to disable parallelism, not
> to share a database. Do not convert it to a shared collection fixture — the ordered CRUD
> tests assume an isolated per-class DB.

### 4. `TableRepositoryTests.cs` — fix `DeleteTable_ShouldReduceCount`

Replace the delete-all/expect-0 body with the delete-the-created-row/expect-1 pattern used
by `ArrangementRepositoryTests` and `MenuRepositoryTests`:
```csharp
[Fact, TestPriority(8)]
public async Task DeleteTable_ShouldReduceCount()
{
    var item = await _repository.GetItemsAsync([2]);
    _ = await _repository.Delete(item);

    var items = await _repository.GetItemsAsync();
    items.Should().NotBeNull().And.HaveCount(1);
}
```

### 5. Minor cleanups (test project only)

- Remove the unused `private readonly ITestOutputHelper _testOutputHelper = testOutputHelper;`
  field from each of the five test classes (it is assigned but never read). If a class no
  longer uses `ITestOutputHelper` at all, also drop the constructor parameter and the
  `using Xunit.Abstractions;` where they become unused. Keep this purely mechanical — do not
  alter any test logic while doing it.
- These are the only "cleanup" edits; do not refactor assertions, ordering, or repository
  calls.

## Out of scope

- **No changes to application code or migrations:** `DebugDiner.Infrastructure` (incl.
  `DataService`, repositories, `QueryConstants`), `DebugDiner.Domain`, and every
  `Migrations/*.sql` stay exactly as-is. In particular, do **not** re-add a `user` seed to
  `User.sql`, do **not** add cascade logic to `TableRepository.Delete`, and do **not**
  change `DataService.Dispose`.
- Not addressing the app-level "known issues" in `STATUS.md` (string-built SQL,
  `UpdatedAt` NULL fallback, startup admin seeding, undisposed readers in repositories,
  etc.). Those are separate future requests.
- No new test cases or coverage expansion — this changeset only stabilizes the existing
  tests. (Adding tests can be a follow-up changeset.)

## Validation

1. Run the suite:
   ```bash
   dotnet test DD.Infra.Test/DD.Infra.Test.csproj --nologo
   ```
   Expect **40 passed / 0 failed**, and **no** `Test Class Cleanup Failure … IOException`
   lines in the output.
2. Run it a second time (and ideally a third) to confirm stability — the failures were
   order/lock sensitive, so repeat runs must stay green:
   ```bash
   dotnet test DD.Infra.Test/DD.Infra.Test.csproj --nologo
   ```
3. Confirm the rest of the solution still builds:
   ```bash
   dotnet build ReservationsCo.slnx
   ```
   Expect build succeeded, 0 errors.
4. Sanity-check that no application files changed:
   ```bash
   git status --short
   ```
   Only files under `DD.Infra.Test/` (and `plan/`) should appear — nothing under
   `DebugDiner.Infrastructure/`, `DebugDiner.Domain/`, or `Migrations/`.

## Handoff prompt (for the implementation session)

> Implement the plan in `plan/01-infrastructure-tests.md`. Follow the project's existing
> conventions and the decisions recorded in `plan/00-decisions.md`. Work through the
> implementation plan in order, then run the validation steps and report the results. Do
> not modify any application code or migrations — all changes stay inside `DD.Infra.Test`.
