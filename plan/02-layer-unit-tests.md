# 02 — Unit tests for Domain, Application, and Presentation layers

## Goal

Add one xUnit + FluentAssertions test project per remaining application layer — `DD.Domain.Test`,
`DD.Application.Test`, `DD.Presentation.Test` — mirroring the existing `DD.Infra.Test`, and
fill each with unit tests for that layer's logic. No production code changes.

## Context

- Source request: `requests/2026-06-unit-testing.md`. It asks to test "the whole of the
  application, each layer in their own unit testing project," with xUnit + FluentAssertions.
  The Infrastructure layer is already covered by `DD.Infra.Test` and is left untouched.
- Decisions for this changeset are in `plan/00-decisions.md` under
  `2026-06-15 — Unit tests for Domain, Application, Presentation`. Read them — they explain
  the naming, the logic-only Presentation scope, the `NavigationService` stub approach, and
  the real-BCrypt choice.
- Baseline architecture is in `plan/STATUS.md`. Key facts that shape these tests:
  - Target framework `net10.0`, `ImplicitUsings` + `Nullable` enabled across the solution.
  - **Entities live in the global namespace** (no `namespace` declaration in `*Entity.cs`),
    as do the enums in `DebugDiner.Domain/Entities/Enums.cs` (`Role`, `ReservationStatus`,
    `TableType`, `DishCategory`, `ArrangementType`). With implicit usings they are reachable
    from test code without a `using`.
  - The existing test project conventions to copy from `DD.Infra.Test/DD.Infra.Test.csproj`:
    `IsPackable=false`, `<Using Include="Xunit"/>`, and package versions
    FluentAssertions 8.9.0, Microsoft.NET.Test.Sdk 17.14.1, Moq 4.20.72, xunit 2.9.3,
    xunit.runner.visualstudio 3.1.4, coverlet.collector 6.0.4, BCrypt.Net-Next 4.1.0.

### What each layer exposes for testing (verified by reading the source)

**Domain (`DebugDiner.Domain`)** — pure, no external dependencies, no mocks:
- `BaseEntity` and the five derived entities (`UserEntity`, `ReservationEntity`,
  `ArrangementEntity`, `TableEntity`, `DishEntity`) each override `Equals`/`GetHashCode` with
  value semantics (base compares `Id`, `CreatedAt`, `UpdatedAt`; derived add their own
  fields). All non-`Dish` fields are `required`; `DishEntity.AllergenInfo` defaults to `""`.
- `RepositoryUtilities.MapToEnum<T>()` — extension on `string` in
  `DebugDiner.Domain.Utilities`; `(T)Enum.Parse(typeof(T), value)` (case-sensitive, throws on
  bad input).
- `DatabaseOptions.ResolvedSource()` in `DebugDiner.Domain.Configurations` — throws
  `InvalidOperationException` when `Source` is null/whitespace; expands `%APPDATA%` to
  `LocalApplicationData`; otherwise `Path.Combine(Environment.CurrentDirectory, Source)`.

**Application (`DebugDiner.Application`)**:
- `AuthService(IUserRepository users, ILogger logger)` (Serilog `ILogger`) with
  `LoginAsync(email, password)` and `RegisterAsync(name, email, password, isAdmin=false)`.
  Both swallow exceptions and return `null`, logging via Serilog. Login is case-insensitive
  on email and verifies with BCrypt. Register rejects duplicate emails (case-insensitive),
  builds a `UserEntity` (`Id=0`, `CreatedAt=UtcNow`, BCrypt hash, `Role` = `Admin` when
  `isAdmin` else `Customer`) and calls `users.Create([entity])`.
- `ServiceExtensions.AddApplication()` registers `IAuthService → AuthService` as a singleton.

**Presentation (`DebugDiner`)** — logic-only (see decisions):
- `NavigationRegistry.GetItemsFor(Type viewType, INavigationService nav)` — returns the
  sidebar `NavigationItem`s for a view type; empty for auth/transient/unknown types;
  `HomeView` adds four admin items when `AppState.CurrentUser?.Role == Role.Admin`; the
  shared `Logout` action sets `AppState.CurrentUser = null`, calls `nav.ClearHistory()` then
  `nav.NavigateTo<WelcomeView>()`.
- `NavigationItem(string Label, Action<INavigationService> Navigate)` — record.
- `AppState` — static mutable holder (`CurrentUser`, `SelectedUser`, `SelectedDish`,
  `SelectedReservation`).
- `NavigationService(IServiceProvider services)` — `SetContentArea`, `NavigateTo<TView>`,
  `NavigateBack`, `ClearHistory`; resolves views from the provider and swaps them into the
  content area's single subview; pushes the previous view type onto a history stack unless it
  is in the hardcoded `_transientViews` set.

## Implementation plan

> Order: build Domain first (no deps), then Application, then Presentation. Add each project
> to the solution as you go. Run `dotnet test` per project before moving on.

### 0. Solution wiring (`ReservationsCo.slnx`)

Add three `<Project>` lines alongside the existing ones:

```xml
<Project Path="DD.Domain.Test/DD.Domain.Test.csproj" />
<Project Path="DD.Application.Test/DD.Application.Test.csproj" />
<Project Path="DD.Presentation.Test/DD.Presentation.Test.csproj" />
```

---

### 1. `DD.Domain.Test`

**`DD.Domain.Test/DD.Domain.Test.csproj`** — model on `DD.Infra.Test.csproj`, trimmed to what
Domain needs (no Moq, no BCrypt, no Sqlite):

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.4"/>
    <PackageReference Include="FluentAssertions" Version="8.9.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1"/>
    <PackageReference Include="xunit" Version="2.9.3"/>
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4"/>
  </ItemGroup>
  <ItemGroup>
    <Using Include="Xunit"/>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\DebugDiner.Domain\DebugDiner.Domain.csproj" />
  </ItemGroup>
</Project>
```

Test files (one per production type; namespace `DD.Domain.Test`):

- **`BaseEntityTests.cs`** — build two `BaseEntity` with identical `Id`/`CreatedAt`/`UpdatedAt`
  and assert `Equals` true + equal `GetHashCode`; flip each of the three fields in turn and
  assert `Equals` false; assert `Equals(null)` and `Equals("not an entity")` are false; assert
  reference-equality short-circuit (`x.Equals(x)` true). Use a fixed `DateTime` (e.g.
  `new DateTime(2024,1,1)`) so `CreatedAt` is deterministic.
- **`UserEntityTests.cs`** — equal users equal; differ in `Name`, `Email`, `PasswordHash`,
  `Role` each → not equal; differ in a base field (`Id`) → not equal (confirms `base.Equals`
  participates); equal hash codes for equal instances; `Equals` against a `BaseEntity` or other
  type → false.
- **`ReservationEntityTests.cs`** — same pattern over `UserId`, `TableId`, `StartTime`. Note
  `Equals`/`GetHashCode` deliberately ignore `EndTime`, `Guests`, `Status` — add an explicit
  test asserting two reservations differing **only** in `Status` are still `Equals` (documents
  the implemented behavior; do not "fix" the entity).
- **`ArrangementEntityTests.cs`** — over `Name`, `BasePrice`, `Type`.
- **`TableEntityTests.cs`** — over `Capacity`, `Type`.
- **`DishEntityTests.cs`** — over `Name`, `Description`, `Price`, `DishCategory`,
  `AllergenInfo`; include a case where `AllergenInfo` is left default `""` on both sides.
- **`RepositoryUtilitiesTests.cs`** (`using DebugDiner.Domain.Utilities;`) — `Theory` mapping
  valid strings to each enum (e.g. `"Admin" → Role.Admin`, `"Confirmed" → ReservationStatus.Confirmed`,
  `"Bar" → TableType.Bar`, `"Vegan" → DishCategory.Vegan`, `"Wine" → ArrangementType.Wine`);
  a `Fact` that an unknown value (`"NotARole".MapToEnum<Role>()`) throws `ArgumentException`;
  a `Fact` documenting case-sensitivity (`"admin".MapToEnum<Role>()` throws).
- **`DatabaseOptionsTests.cs`** (`using DebugDiner.Domain.Configurations;`) — `new DatabaseOptions()`
  (Source null) `.ResolvedSource()` throws `InvalidOperationException`; `Source = "   "` likewise;
  `Source = "test.db"` returns `Path.Combine(Environment.CurrentDirectory, "test.db")`;
  `Source = "%APPDATA%/x.db"` returns a path that does **not** contain `%APPDATA%` and starts
  with `Environment.GetFolderPath(LocalApplicationData)`.

A small shared helper for building entities is optional; inline object initializers are fine
and match the Infra test style. Prefer FluentAssertions throughout
(`x.Should().Be(...)`, `act.Should().Throw<...>()`, `dict.Should().Equal(...)`).

---

### 2. `DD.Application.Test`

**`DD.Application.Test/DD.Application.Test.csproj`** — adds Moq, BCrypt, and
`Microsoft.Extensions.DependencyInjection` (for the `AddApplication` test). Serilog `ILogger`
and the entity types come transitively through the Application project reference.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="BCrypt.Net-Next" Version="4.1.0" />
    <PackageReference Include="coverlet.collector" Version="6.0.4"/>
    <PackageReference Include="FluentAssertions" Version="8.9.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.5" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1"/>
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="xunit" Version="2.9.3"/>
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4"/>
  </ItemGroup>
  <ItemGroup>
    <Using Include="Xunit"/>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\DebugDiner.Application\DebugDiner.Application.csproj" />
  </ItemGroup>
</Project>
```

Test files (`namespace DD.Application.Test`; `using DebugDiner.Application;`,
`using DebugDiner.Domain.Abstractions;`, `using Moq;`, `using Serilog;`,
`using BC = BCrypt.Net.BCrypt;`):

- **`AuthServiceTests.cs`** — construct `AuthService` with `Mock<IUserRepository>` and
  `Mock<ILogger>`. Helper to build a `UserEntity` with a real hash:
  `PasswordHash = BC.HashPassword("secret")`. Cover:
  - **Login success** — repo `GetItemsAsync()` returns a list containing a user whose hash
    matches; `LoginAsync(email, "secret")` returns that user.
  - **Login email case-insensitive** — stored email `"User@Example.com"`, log in with
    `"user@example.com"` → returns the user.
  - **Login unknown email** — repo returns users without a match → returns `null`.
  - **Login wrong password** — matching email but `LoginAsync(email, "wrong")` → `null`.
  - **Login repo throws** — `GetItemsAsync` set up to throw → returns `null` (exception
    swallowed), and optionally `logger.Verify(l => l.Error(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object>()), Times.Once)`.
  - **Register success** — repo `GetItemsAsync()` returns empty; set up `Create` to echo its
    argument; capture the created entity via a Moq `Callback`. Assert returned user non-null,
    `Role == Role.Customer`, `Id == 0`, `Email`/`Name` propagated, and
    `BC.Verify("secret", entity.PasswordHash)` is true (password is hashed, not stored plain).
  - **Register as admin** — `isAdmin: true` → captured entity `Role == Role.Admin`.
  - **Register duplicate email (case-insensitive)** — repo returns a user with the same email
    in different casing → returns `null` and `Create` is never called
    (`users.Verify(u => u.Create(It.IsAny<IEnumerable<UserEntity>>()), Times.Never)`).
  - **Register repo throws** — `Create` (or `GetItemsAsync`) throws → returns `null`.

  Keep Serilog verification light/optional (its `Warning`/`Error` generic overloads are
  awkward to match precisely in Moq); assert on return values and on `IUserRepository`
  interactions as the primary signal.
- **`ServiceExtensionsTests.cs`** (`using Microsoft.Extensions.DependencyInjection;`) — call
  `new ServiceCollection().AddApplication()`, also register `Mock<IUserRepository>().Object`
  and `Mock<ILogger>().Object`, `BuildServiceProvider()`, then
  `GetRequiredService<IAuthService>().Should().BeOfType<AuthService>()`. Also assert the
  registration lifetime is `Singleton` by inspecting the `ServiceDescriptor`.

---

### 3. `DD.Presentation.Test`

**`DD.Presentation.Test/DD.Presentation.Test.csproj`** — references the `DebugDiner` host
project (allowed even though it is `OutputType=Exe`); Terminal.Gui and Serilog flow in
transitively. Adds Moq and `Microsoft.Extensions.DependencyInjection`.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="coverlet.collector" Version="6.0.4"/>
    <PackageReference Include="FluentAssertions" Version="8.9.0" />
    <PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="10.0.5" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.14.1"/>
    <PackageReference Include="Moq" Version="4.20.72" />
    <PackageReference Include="xunit" Version="2.9.3"/>
    <PackageReference Include="xunit.runner.visualstudio" Version="3.1.4"/>
  </ItemGroup>
  <ItemGroup>
    <Using Include="Xunit"/>
  </ItemGroup>
  <ItemGroup>
    <ProjectReference Include="..\DebugDiner\DebugDiner.csproj" />
  </ItemGroup>
</Project>
```

Common usings in these tests: `using DebugDiner;`, `using DebugDiner.Services;`,
`using Terminal.Gui;`, `using Moq;`.

- **`Stubs/StubViews.cs`** — minimal `Terminal.Gui` views for the `NavigationService` tests:
  ```csharp
  using Terminal.Gui;
  namespace DD.Presentation.Test;
  public sealed class StubViewA : View;
  public sealed class StubViewB : View;
  public sealed class StubViewC : View;
  ```
  Plus a small helper building an `IServiceProvider` that can resolve them transiently
  (`new ServiceCollection().AddTransient<StubViewA>()...BuildServiceProvider()`).

- **`AppStateTests.cs`** — set/get round-trip for `CurrentUser`, `SelectedUser`,
  `SelectedDish`, `SelectedReservation`; setting back to `null` clears. Reset all four to
  `null` at the end of each test (static state). Build entities with the required-member
  initializers.

- **`NavigationItemTests.cs`** — construct a `NavigationItem("Home", n => ...)`; assert
  `Label`; invoke `Navigate(mockNav.Object)` and verify the action ran (e.g. it calls a
  `Mock<INavigationService>` method). Optionally assert record value-equality for two items
  with the same `Label` + same delegate instance.

- **`NavigationRegistryTests.cs`** — `using` static `AppState`. Because these mutate
  `AppState.CurrentUser`, set it explicitly in each test's arrange and reset to `null` in a
  `Dispose`/finally; keep all `AppState`-dependent tests in this single class so they never
  interleave (xUnit serializes methods within a class). Cover:
  - Auth/transient types (`WelcomeView`, `LoginView`, `RegisterView`, `CreateUserView`,
    `UpdateUserView`, `DeleteUserView`, `CreateDishView`, `UpdateDishView`, `DeleteDishView`,
    `CreateReservationsView`, `UpdateReservationView`, `DeleteReservationView`) → empty.
  - Unknown type (e.g. `typeof(string)`) → empty.
  - `HomeView` with `AppState.CurrentUser = null` → 6 items, labels `Home`,
    `Make a Reservation`, `View my Reservations`, `User Information`, `Menu (Dishes)`,
    `Logout`; no admin items.
  - `HomeView` with a `Customer` user → same 6 items (admin block skipped).
  - `HomeView` with an `Admin` user → 10 items, including `Create Dish`, `Users`, `Add User`,
    `Admin Reservations`.
  - `ReservationsView`, `InformationView`, `DishView`, `AdminUsersView` → assert exact label
    sets per `NavigationRegistry`.
  - **Logout action** — for the `HomeView`/`Customer` list, grab the `Logout` item, set
    `AppState.CurrentUser` to a non-null user, invoke `item.Navigate(mockNav.Object)`; assert
    `AppState.CurrentUser` is now `null`, and `mockNav.Verify(n => n.ClearHistory())` +
    `mockNav.Verify(n => n.NavigateTo<WelcomeView>())` each once.

- **`NavigationServiceTests.cs`** — build a provider that resolves `StubViewA/B/C`, construct
  `new NavigationService(provider)`, and observe behavior through the content area's
  `Subviews`:
  - **No content area** — without `SetContentArea`, `NavigateTo<StubViewA>()` does not throw
    and is a no-op.
  - **NavigateTo swaps content** — `SetContentArea(new View())`; `NavigateTo<StubViewA>()`;
    the content area has exactly one subview and it is a `StubViewA`.
  - **NavigateTo replaces** — then `NavigateTo<StubViewB>()`; content area still has exactly
    one subview, now a `StubViewB` (old removed).
  - **Back restores previous** — after A then B, `NavigateBack()` → content area's subview is
    a `StubViewA` again (a fresh instance; assert by type).
  - **Back with empty history is a no-op** — single `NavigateTo<StubViewA>()` then
    `NavigateBack()` → still a `StubViewA`, no throw.
  - **ClearHistory** — A, B, `ClearHistory()`, `NavigateBack()` → content stays `StubViewB`.
  - **NavigationItemsChanged fires** — subscribe; `NavigateTo<StubViewA>()` raises the event
    (payload is empty because stub types aren't in the registry — assert it fired, not its
    contents).

  > **Driver note for the implementer:** plain `Terminal.Gui.View` `Add`/`Remove`/`Subviews`
  > and property sets are expected to work without `Application.Init()`. If any call throws
  > because a driver is required, wrap these tests in an `IClassFixture` that calls
  > `Application.Init()` in its constructor and `Application.Shutdown()` in `Dispose`, and put
  > the class in its own `[Collection]` (Terminal.Gui `Application` is global static, so such
  > tests must not run in parallel with each other). Do **not** change `NavigationService`.

## Out of scope

- **No production code changes.** `DebugDiner`, `DebugDiner.Application`, `DebugDiner.Domain`,
  `DebugDiner.Infrastructure`, and all migrations stay exactly as-is. If a test reveals a bug
  (e.g. `ReservationEntity` equality ignoring `Status`), document it in the test as the
  *current* behavior — fixing it is a separate request per the Phase 5 rule in `CLAUDE.md`.
- **No changes to `DD.Infra.Test`** — it is already covered by `plan/01`.
- **Terminal.Gui `View` subclasses** (`LoginView`, `HomeView`, `WelcomeView`, all CRUD views,
  `BaseView`, `LayoutView`) are not unit-tested — they need a live UI driver (decision logged
  in `00-decisions.md`).
- **The `_transientViews` exclusion branch** of `NavigationService` (history skipping for
  transient CRUD views) is not asserted — it is keyed on concrete real view types that can't
  be driven by stubs without the UI stack.
- No `Program.Main`/host-wiring tests, no `DataService`/repository tests (those are Infra),
  and no coverage-percentage target — the goal is meaningful per-type unit tests, not a number.

## Validation

Run from the repo root. Each new project should build and pass on its own:

```bash
dotnet test DD.Domain.Test/DD.Domain.Test.csproj --nologo
dotnet test DD.Application.Test/DD.Application.Test.csproj --nologo
dotnet test DD.Presentation.Test/DD.Presentation.Test.csproj --nologo
```

Then confirm the whole solution still builds and the full suite (including the existing
`DD.Infra.Test`) is green:

```bash
dotnet build ReservationsCo.slnx           # expect: Build succeeded, 0 errors
dotnet test  ReservationsCo.slnx --nologo  # expect: all tests passed (40 Infra + the new ones)
```

Finally, confirm only test projects and `plan/` changed — nothing under the four production
projects or `Migrations/`:

```bash
git status --short
```

## Handoff prompt (for the implementation session)

> Implement the plan in `plan/02-layer-unit-tests.md`. Follow the project's existing
> conventions (copy `DD.Infra.Test`'s style) and the decisions recorded in
> `plan/00-decisions.md`. Create the three test projects in order (Domain → Application →
> Presentation), add them to `ReservationsCo.slnx`, then run the validation steps and report
> the results. Do not modify any production code — if a test surfaces a bug, document the
> current behavior in the test rather than changing the app.
