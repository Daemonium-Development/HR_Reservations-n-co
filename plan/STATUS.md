# Current Status — Debug Diner (Reservations-n-co)

**Snapshot date:** 2026-06-15 · **Branch:** `main` · **Last commit:** `d7b7a7b`

This is the agreed *starting point* for the plan-then-implement workflow described in
`CLAUDE.md`. It is a point-in-time snapshot, not a contract — when something here changes,
record the *why* in `plan/00-decisions.md` and (if part of a changeset) the *what* in a
`plan/NN-*.md` file. The architectural rationale behind the state below lives in the
`2026-06-15 — Baseline` entry of `plan/00-decisions.md`.

---

## What the application is

"Debug Diner" is a restaurant **reservation system** built as a Terminal.Gui (TUI) console
app for a Hogeschool Rotterdam SCRUM project. Users register/log in, browse the menu, and
make/view/edit reservations; admins manage users, dishes, and all reservations.

## Solution layout

| Project | Role | Key types |
|---|---|---|
| `DebugDiner.Domain` | Entities, enums, abstractions, options | `*Entity`, `I*Repository`, `IDataService`, `DatabaseOptions`, `Role`/`ReservationStatus`/`TableType`/`DishCategory`/`ArrangementType` |
| `DebugDiner.Application` | Use-case services | `AuthService` / `IAuthService` (BCrypt login + register) |
| `DebugDiner.Infrastructure` | SQLite persistence | `DataService`, repositories, `QueryConstants`, embedded `Migrations/*.sql` |
| `DebugDiner` | TUI host + presentation | `Program`, `AppState`, `NavigationService`/`Registry`, `Views/*` |
| `DD.Infra.Test` | xUnit integration tests | repository tests + `DatabaseFixture` |
| `Documentation` | SCRUM/course docs (markdown + diagrams) | backlogs, reviews, ERD, DoD |

- **Target framework:** `net10.0`, nullable + implicit usings enabled.
- **Solution file:** `ReservationsCo.slnx`.
- **Key packages:** Terminal.Gui 1.19, Microsoft.Extensions.Hosting 10, Microsoft.Data.Sqlite,
  Serilog (+ File sink), BCrypt.Net, xUnit + FluentAssertions + Moq.

## Domain model & schema

Entities derive from `BaseEntity` (`Id`, `CreatedAt`, `UpdatedAt`) and carry value-equality
overrides. Tables are created at startup from embedded SQL in dependency order:
`user → arrangement → dish(Menu.sql) → reservation → table → arrangement_dish → reservation_arrangement`.

- `user` (name, email, password_hash, role)
- `dish` (name, price, description, category, allergen_info) — entity is `DishEntity`, repo is `MenuRepository`/`IMenuRepository`
- `arrangement` (name, base_price, type)
- `table` (capacity, type)
- `reservation` (user_id→user, table_id→table, start/end time, guests, status)
- `arrangement_dish` (dish_id, arrangement_id) — join
- `reservation_arrangement` (reservation_id, arrangement_id) — join

Each entity table has an `updated_at` trigger; most seed one demo row. The `user` table is
**not** seeded — instead `Program.Main` seeds an admin (`Soufian / soufian@gmail.com / 1234`)
on every startup via `AuthService.RegisterAsync` (a dev convenience, commented teammates).

## Implemented features (vs. product backlog)

The backlog (`Documentation/.../Product Backlog - User Stories.md`) lists 10 stories. Views
exist for the full set of flows:

- **Auth:** Welcome → Login / Register (BCrypt). Logout clears `AppState` and nav history.
- **Reservations:** create / view / update / delete (`CreateReservationsView`,
  `ReservationsView`, `UpdateReservationsView`, `DeleteReservationsView`).
- **Menu/dishes:** view + admin create/update/delete (`DishView`, `*DishView`).
- **User info:** `InformationView`.
- **Admin:** user list + create/update/delete (`AdminUsersView`, `*UserView`); admin
  reservation management; admin-only sidebar items gated on `Role.Admin`.

> Mapping of each backlog story to "done / partial / missing" has **not** been formally
> verified against the running UI — treat the above as "screens exist", not "fully works".
> A first useful request could be a feature-completeness audit.

## Build & test health

- **Build:** `dotnet build ReservationsCo.slnx` → **succeeds** (a few first-build warnings,
  0 errors).
- **Tests:** `dotnet test ReservationsCo.slnx` → **33 passed / 7 failed / 40 total.**
  Failures are in `ReservationRepositoryTests` and `UserRepositoryTests` (e.g.
  `UpdateReservation_ShouldSucceed`, `GetAllUsers_ShouldSucceed` returning 0 rows) plus
  `Test Class Cleanup Failure … System.IO.IOException`. They appear **order-dependent and
  Windows file-lock related** (shared `IClassFixture` + `TestPriority` ordering + WAL files
  held open when the fixture tries to delete the `.db`), not core SQL logic bugs — but this
  is unconfirmed and is the most concrete thing to investigate first.

## Known issues / rough edges (candidates for future requests)

1. **Flaky tests** — 7/40 fail as above; stabilize fixture isolation / WAL cleanup / ordering.
2. **Startup admin seeding** — `Program.Main` always registers an admin user; should be
   guarded or moved out of the hot path before any "production" use.
3. **SQL built via string replacement** — `{table}`/`{columns}` are interpolated. Safe today
   (hard-coded, trusted) but brittle; values are parameterized correctly.
4. **`DatabaseOptions.Source`** is non-nullable but unset until config binds; `ResolvedSource`
   guards with a throw. Minor nullability smell.
5. **`UpdatedAt` fallback** — readers substitute `DateTime.Now` when `updated_at` is NULL
   rather than `null`, slightly misrepresenting "never updated".
6. **Cross-platform paths** — log path and an `appsettings` note are hand-toggled for Mac vs.
   Windows via comments.
7. **No application-layer services beyond auth** — repositories are called paths;
   reservation/business rules (overlap checks, capacity vs. guests, table availability) are
   not centrally enforced.

## How to run / validate locally

```bash
# one-time: create the local config from the template
cp DebugDiner/appsettings.template.json DebugDiner/appsettings.json

dotnet build ReservationsCo.slnx          # expect: Build succeeded
dotnet run --project DebugDiner           # launches the TUI (DB self-creates under %LocalAppData%/Debug Diner)
dotnet test  ReservationsCo.slnx          # expect (today): 33 passed / 7 failed
```

## Workflow from here

New work starts with a markdown file in `requests/`, then a planning pass that produces a
`plan/NN-<slug>.md` changeset plus a `plan/00-decisions.md` entry — per `CLAUDE.md`. This
`STATUS.md` is the reference baseline those plans build on; update it as the project moves.
