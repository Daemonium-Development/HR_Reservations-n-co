# Decisions Log

Append-only running log of decisions across **all** changesets in this project.
Newest entries go at the bottom. Never rewrite or delete earlier entries — this is a
historical record, not a snapshot. See `plan/STATUS.md` for a point-in-time snapshot of
the application as a whole.

---

## 2026-06-15 — Baseline (see plan/STATUS.md)

This first entry captures architectural decisions that were **already embedded in the
codebase** when the plan-then-implement workflow was adopted. They are recorded here so
future changesets have a documented "why" to build on. No code was changed to produce
this entry.

- **Decision:** Clean/onion architecture split across five projects — `DebugDiner.Domain`
  (entities, enums, repository & service abstractions, options), `DebugDiner.Application`
  (use-case services such as auth), `DebugDiner.Infrastructure` (SQLite data access,
  embedded SQL schema, repositories), `DebugDiner` (Terminal.Gui presentation/host), and
  `DD.Infra.Test` (xUnit integration tests). `Documentation` is a docs-only project.
  **Why:** Keeps domain rules independent of UI and persistence; dependencies point
  inward toward the domain.

- **Decision:** Terminal.Gui 1.19 TUI as the presentation layer, hosted via
  `Microsoft.Extensions.Hosting` generic host with DI.
  **Why:** Console/TUI was the agreed delivery format for this restaurant-reservation
  school project; the generic host gives DI, configuration, and logging for free.

- **Decision:** Raw ADO.NET against SQLite (`Microsoft.Data.Sqlite`) with hand-written
  SQL, rather than an ORM (EF Core/Dapper). Queries are built from string templates in
  `QueryConstants` with `{table}`/`{columns}`/`{values}` placeholders; values are passed
  as parameters.
  **Why:** Educational goal of working close to SQL. Note: table/column names are
  injected via string replacement (trusted, hard-coded), while user-supplied values use
  parameters — see STATUS "Known issues" for the caveat.

- **Decision:** Schema is applied at startup from embedded `.sql` resources in
  `DebugDiner.Infrastructure/Migrations`, run in FK-dependency order with
  `PRAGMA foreign_keys=OFF` during the run, using `CREATE TABLE IF NOT EXISTS`. WAL +
  `synchronous=NORMAL` are enabled. Several scripts also seed a demo row and an
  `updated_at` trigger.
  **Why:** Zero-setup local database that self-creates on first run; idempotent across
  restarts.

- **Decision:** Database location resolves from `Database:Source` in `appsettings.json`;
  `%APPDATA%` expands to `LocalApplicationData`, otherwise the path is treated as relative
  to the current directory. `appsettings.json` is git-ignored and created from
  `appsettings.template.json`.
  **Why:** Per-user local database without committing machine-specific paths or secrets.

- **Decision:** Passwords stored as BCrypt hashes; auth lives in
  `DebugDiner.Application.AuthService`. There is no session/token concept — the signed-in
  user is held in the static `AppState.CurrentUser`, and selection state for CRUD screens
  in `AppState.Selected*`.
  **Why:** Single-user desktop TUI; static app state is sufficient for navigation between
  views.

- **Decision:** Navigation is centralized in `NavigationService` (forward stack +
  back/clear) with a static `NavigationRegistry` mapping each view type to its sidebar
  items, and a set of "transient" CRUD views that are excluded from back-history.
  Role-gated items (Admin) are added conditionally.
  **Why:** Keeps per-view wiring out of the views and gives one place to define menus and
  role visibility.

- **Decision:** Logging via Serilog to a rolling daily file under
  `LocalApplicationData/Debug Diner/logs` (Mac path noted inline as an alternative).
  **Why:** TUI owns the console, so file logging is the practical sink for diagnostics.

- **Decision:** Repositories registered as singletons; `IDataService` owns a single
  shared `SqliteConnection` opened at startup.
  **Why:** Simple lifetime model for a single-user app with one long-lived connection.

- **Decision:** Tests are integration tests that exercise real repositories against a
  throwaway on-disk SQLite file per fixture, ordered with a custom `TestPriority` orderer.
  **Why:** The value under test is the hand-written SQL, which only a real database can
  validate. (Ordering + shared fixture currently causes flakiness — see STATUS.)

---

## 2026-06-15 — Stabilize infrastructure tests (see plan/01-infrastructure-tests.md)

Fixes the 7/40 failing `DD.Infra.Test` integration tests. All changes are confined to the
`DD.Infra.Test` project; `DebugDiner.Infrastructure`, `DebugDiner.Domain`, and the
`Migrations/*.sql` are left untouched, per the request's rule "adjusting the application to
fit tests is not done today — how things stand is how things work"
(`requests/2026-06-infrastructure-test.md`).

- **Decision:** The missing `user` seed row (`id=1`) is created by the test
  `DatabaseFixture` via a raw `INSERT` on the live connection right after `StartAsync`,
  **not** by re-adding an `INSERT` to `User.sql`.
  **Why:** The app deliberately leaves `user` unseeded in migrations and instead seeds an
  admin at runtime in `Program.Main` via `AuthService.RegisterAsync` (see STATUS). The
  `DataService`-only fixture bypasses `Program.Main`, so the precondition (a user exists)
  is absent. Seeding in the fixture reproduces the real runtime precondition without
  changing application behavior. This single fix resolves the User Get/Update failures and
  the Reservation Create/Update FK failures (the seeded reservation references
  `user_id=1`, and inserts run with `PRAGMA foreign_keys=ON`).

- **Decision:** `TableRepositoryTests.DeleteTable_ShouldReduceCount` is changed to delete
  only the created table (`id=2`) and assert the remaining count is `1`, mirroring the
  `Arrangement`/`Menu` delete tests, instead of deleting all tables and expecting `0`.
  **Why:** `TableRepository.Delete` does not cascade, so the seeded reservation's FK to
  `table(id=1)` blocks deleting table 1 — delete-all can never reach `0` under current
  infrastructure. (Contrast `UserRepository.Delete`, which *does* cascade reservations, so
  the User delete-all→0 test stays valid.) Adjusting the test, not the repository, honors
  the no-app-changes rule.

- **Decision:** `DatabaseFixture.Dispose` calls `SqliteConnection.ClearAllPools()` (plus a
  GC drain) before deleting the DB file, and also removes the `-wal`/`-shm` sidecar files
  with a short retry.
  **Why:** Microsoft.Data.Sqlite pools connections by default, so disposing the
  `DataService` connection returns the handle to the pool instead of releasing the OS file
  lock — causing the `System.IO.IOException` "Test Class Cleanup Failure" on Windows.
  `ClearAllPools` is global static, callable from the fixture without touching the app.

- **Decision:** All five repository test classes are placed in the existing
  `[Collection("Database")]` (activating the previously-dead `DatabaseTestCollection`) so
  they run serially.
  **Why:** `ClearAllPools()` is process-global; if classes ran in parallel, one class's
  teardown could close another class's in-use pooled connection mid-test. Serializing
  removes that interference and explains why the collection was defined.

---

## 2026-06-15 — Unit tests for Domain, Application, Presentation (see plan/02-layer-unit-tests.md)

Adds one xUnit + FluentAssertions test project per remaining layer, mirroring the existing
`DD.Infra.Test`. Source request: `requests/2026-06-unit-testing.md`. No application code is
changed — only new test projects and the `.slnx` are added.

- **Decision:** New test projects are named `DD.Domain.Test`, `DD.Application.Test`, and
  `DD.Presentation.Test` — the dotted `DD.<Layer>.Test` form.
  **Why:** Matches the one existing test project (`DD.Infra.Test`) for consistency in test
  discovery and naming. The request's literal names (`DD.DomainTests`, etc.) were
  approximate — the request itself calls the infra project `DD.InfraTests`, which does not
  match the actual `DD.Infra.Test`. User confirmed the dotted convention.

- **Decision:** The Presentation (`DebugDiner`) layer is tested **logic-only**: cover
  `NavigationRegistry`, `NavigationItem`, `AppState`, and `NavigationService`. The
  Terminal.Gui `View` subclasses (`LoginView`, `HomeView`, …) are **out of scope**.
  **Why:** The views require a live Terminal.Gui driver (`Application.Init`) and build full
  UI trees in their constructors; they are integration/UI concerns, not unit-testable in
  isolation without a fragile headless-console setup. The genuine, driver-independent logic
  lives in the navigation/registry/state types. User chose "logic-only (recommended)".

- **Decision:** `NavigationService` is tested with lightweight stub `View` subclasses
  registered in a real `ServiceCollection`, and behaviour is observed through the public
  `contentArea.Subviews` (the current view) rather than its private fields.
  **Why:** `NavigateTo`/`NavigateBack` resolve views from the `IServiceProvider` and swap
  them into the content area; stubs avoid pulling in the real UI stack while still
  exercising the forward/back/clear-history logic. The `_transientViews` exclusion branch
  references concrete real view types and cannot be driven by stubs, so that one branch is
  left out of scope (documented in the plan).

- **Decision:** `AuthService` tests use **real BCrypt** hashing (not a mocked hasher) with
  `IUserRepository` and Serilog `ILogger` mocked via Moq.
  **Why:** BCrypt is a pure, fast, deterministic-enough dependency already referenced by the
  Application project; mocking it would add an abstraction the app does not have. Assertions
  focus on returned `UserEntity`/`null` and the entity handed to `IUserRepository.Create`;
  Serilog log-call verification is kept light because its generic overloads are awkward to
  match in Moq.

---

## 2026-06-15 — Translate Team documentation to Dutch (see plan/03-translate-team-docs-to-dutch.md)

Brings every prose document under `Documentation/Team` to consistent, natural Dutch. Source
request: `requests/2026-06-documentation.md`. Only docs under `Documentation/Team` are
touched; `Documentation/Individual` is explicitly left alone (it is already done and serves
only as a writing-style reference). No source code, build files, or binary assets are changed.

- **Decision:** Established Scrum/Agile terms stay in English (Definition of Done, Sprint,
  Sprint Review, Sprint Planning, Daily Stand-up, Retrospective, Product Backlog, User Story,
  Scrum Master).
  **Why:** They are standard terminology and already written this way in the Individual docs
  that serve as the style reference; translating them would be less conventional and
  inconsistent across the project. User confirmed.

- **Decision:** Component/view/repository/class/story/task names that double as identifiers
  stay in English as proper names (e.g. `ReservationsView`, `UserRepository`, "Information
  View", "User login", the "Opgeleverde taken" task lists, the `### <Story>` headings, PR /
  branch names). Only genuinely descriptive prose around them is translated.
  **Why:** These map to code symbols and to cross-references in other docs; translating them
  would break consistency with the codebase and with the Individual docs. User confirmed.

- **Decision:** Verbatim factual content keeps its exact wording: commit messages and author
  names in the sprint tables, code identifiers, file paths, GitHub URLs, enum values, and
  literal in-app strings (backtick UI text such as `"All fields are required."`, status names
  such as `Pending`, and the `✅ AVAILABLE` / `❌ TAKEN` labels).
  **Why:** These are a historical record or reflect what the running application actually
  outputs (the app's UI is English); translating them would misrepresent reality. User
  confirmed.

- **Decision:** Em-dashes (and em-dash-style en-dashes) are stripped **everywhere** in the
  Team files, including inside commit messages and code-comment lines, and replaced with a
  plain hyphen (or the sentence is restructured). This is the one mechanical change applied
  even to otherwise-verbatim content.
  **Why:** The request bans em-dashes as an "AI tell"; the user chose strip-everywhere over
  edited-prose-only. Visible, meaningful arrows (`→`, `⇒`, `↔`) are not dashes and are kept.

- **Decision:** All output uses clean ASCII-style punctuation: straight quotes/apostrophes
  (`'` `"`), no non-breaking spaces, zero-width characters, or stray BOMs. Existing
  authored-Dutch content (including any pre-existing typos) is left intact except for the
  English-to-Dutch translation and the character cleanup above.
  **Why:** The request forbids "invisible (or less visible) bits and bytes" and clearly-AI
  writing; the task is a translation/cleanup, not a content rewrite.
