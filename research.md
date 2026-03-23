# DebugDiner — Solution Research Report

## 1. Solution Overview

**Project Name:** ReservationsCo (branded as DebugDiner)
**Type:** C# Console Application
**Runtime:** .NET 10.0
**Database:** SQLite (local file `database.db`)
**Stage:** Early implementation — infrastructure initialized, business logic absent

This is a restaurant reservation management system. It manages tables, dishes, arrangements (set menus), reservations, and users with role-based access. The application is structured following Clean Architecture principles split across four projects.

---

## 2. Project Structure

```
ReservationsCo.slnx
├── DebugDiner                    (Presentation — Console entry point)
├── DebugDiner.Domain             (Domain — Interfaces, enums, config models)
├── DebugDiner.Application        (Application — Business logic, empty)
└── DebugDiner.Infrastructure     (Infrastructure — SQLite data service, migrations)
```

### Dependency Graph

```
DebugDiner
  └─ references DebugDiner.Domain
  └─ references DebugDiner.Infrastructure

DebugDiner.Infrastructure
  └─ references DebugDiner.Domain

DebugDiner.Application
  └─ references DebugDiner.Domain

DebugDiner.Domain
  └─ no external project references
```

The Domain layer is a clean core with no upward dependencies. Infrastructure depends on Domain to implement its abstractions. The console project wires everything together at the composition root.

---

## 3. Layer-by-Layer Breakdown

### 3.1 Domain Layer — `DebugDiner.Domain`

The purest layer. Contains no NuGet dependencies beyond `Microsoft.Data.Sqlite.Core` (for the `SqliteConnection` type used in the interface).

**`DebugDiner.Domain.Abstractions.ServiceStatus` (enum)**
```
Starting | Running | Stopped | Error
```
Tracks the lifecycle state of the data service connection.

**`DebugDiner.Domain.Abstractions.IDataService` (interface)**
```csharp
ServiceStatus Status { get; }
Task<SqliteConnection?> StartAsync();
Task StopAsync();
Task<SqliteConnection?> RestartAsync();
Task RefreshAsync();
```
Abstracts the database lifecycle. Returns a live `SqliteConnection` so consumers can execute queries directly. Implements `IDisposable`.

**`DebugDiner.Domain.Configurations.DatabaseOptions` (POCO)**
```csharp
public static string SectionName = "Database";
public string Source { get; set; }
public string ResolvedSource();
```
Bound from `appsettings.json`. `ResolvedSource()` expands `%APPDATA%` in the path string, enabling both development (relative path) and production (user-profile path) configs from the same class.

---

### 3.2 Application Layer — `DebugDiner.Application`

**Status: Empty.** The file `DataService.cs` exists but has 0 lines. This layer is intended for orchestration services, use-cases, or CQRS commands/queries, but none have been implemented yet.

---

### 3.3 Infrastructure Layer — `DebugDiner.Infrastructure`

The only layer with real implementation.

#### `DataService` — `DebugDiner.Infrastructure.Services.DataService`

Implements `IDataService`. Constructor-injected with `IOptions<DatabaseOptions>` and `ILogger` (Serilog).

| Method | What it does |
|---|---|
| `StartAsync()` | Opens the SQLite connection, creates the directory if missing, applies PRAGMA settings (`WAL` journal mode, `synchronous=NORMAL`), then runs all schema migrations. Returns the open connection or `null` on failure. Sets `Status = Running`. |
| `StopAsync()` | Closes and disposes the connection. Sets `Status = Stopped`. |
| `RestartAsync()` | Calls `StopAsync()` then `StartAsync()`. |
| `RefreshAsync()` | Re-runs schema migrations without reopening the connection. Useful for applying changes to a live connection. |
| `RunSchemaAsync()` (private) | Disables foreign keys (`PRAGMA foreign_keys = OFF`), iterates all embedded `.sql` resources, executes each, then re-enables foreign keys. |
| `ReadEmbeddedSql()` (private, static) | Uses reflection (`Assembly.GetManifestResourceStream`) to read SQL files compiled into the assembly as embedded resources. |

**PRAGMA settings applied on start:**
```sql
PRAGMA journal_mode = WAL;
PRAGMA synchronous = NORMAL;
```
WAL (Write-Ahead Logging) allows concurrent reads during writes — important for a multi-user reservation system. `synchronous=NORMAL` is a performance/durability trade-off safer than `OFF` but faster than `FULL`.

#### `ServiceExtensions` — `DebugDiner.Infrastructure.ServiceExtensions`

Provides the `AddInfrastructure()` DI registration method. Registers:
- `IOptions<DatabaseOptions>` bound to the `"Database"` config section
- `IDataService` → `DataService` as a **singleton**

> **Note:** This file currently uses an experimental `extension(...)` syntax that is not valid in C# 10/12. The correct syntax is `public static IServiceCollection AddInfrastructure(this IServiceCollection services)`. This is a **compile-blocking bug**.

#### `Migrations/` — Embedded SQL Resources

All `.sql` files are compiled into the assembly via:
```xml
<EmbeddedResource Include="Migrations\*.sql" />
```
They are run on every `StartAsync()` and `RefreshAsync()` call. Because every file starts with `DROP TABLE IF EXISTS`, the schema is fully recreated (and re-seeded) on each startup. This is intentional for the current development phase but will destroy data in production.

---

### 3.4 Presentation Layer — `DebugDiner`

#### `Program.cs`

The composition root. Uses `IHostBuilder` from `Microsoft.Extensions.Hosting`:

1. Loads `appsettings.json` and `appsettings.{Environment}.json` (environment forced to `"Development"`)
2. Configures Serilog:
   - Minimum level: `Debug` (app), `Warning` (Microsoft namespaces)
   - Sink: rolling file at `logs/debug-diner.log` (daily, 30-day retention)
   - Output template: `[{Timestamp} {Level}] {SourceContext}: {Message}{NewLine}{Exception}`
3. Calls `services.AddInfrastructure()` to register data services
4. Resolves `IDataService`, calls `StartAsync()`, and prints the status

Current output behavior: starts the DB, prints `Running` (or the enum value), then exits. No interactive UI implemented yet.

---

## 4. Database Schema

Seven tables in SQLite. All use `INTEGER PRIMARY KEY AUTOINCREMENT` as surrogate keys and `TEXT` for timestamps (SQLite has no native datetime type).

### `user`
| Column | Type | Constraints |
|---|---|---|
| `id` | INTEGER | PK, AUTOINCREMENT |
| `name` | TEXT | NOT NULL |
| `email` | TEXT | NOT NULL |
| `password_hash` | TEXT | NOT NULL |
| `role` | TEXT | NOT NULL, CHECK IN ('Admin', 'Employee', 'Customer') |
| `created_at` | TEXT | NOT NULL |
| `updated_at` | TEXT | NULL |

Seed: Admin user `Soufian Manai`.

### `table`
| Column | Type | Constraints |
|---|---|---|
| `id` | INTEGER | PK, AUTOINCREMENT |
| `capacity` | INTEGER | NOT NULL |
| `type` | TEXT | NOT NULL, CHECK IN ('TwoPerson', 'FourPerson', 'SixPerson', 'Bar') |
| `created_at` | TEXT | NOT NULL |
| `updated_at` | TEXT | NULL |

Seed: 1 Bar table (capacity 4). According to project docs, real data would be 8×TwoPerson, 5×FourPerson, 2×SixPerson, 1×Bar (8 seats).

### `dish`
| Column | Type | Constraints |
|---|---|---|
| `id` | INTEGER | PK, AUTOINCREMENT |
| `name` | TEXT | NOT NULL |
| `price` | REAL | NOT NULL |
| `description` | TEXT | NOT NULL |
| `category` | TEXT | NOT NULL, CHECK IN ('Meat', 'Fish', 'Vegetarian', 'Vegan') |
| `allergen_info` | TEXT | NOT NULL |
| `created_at` | TEXT | NOT NULL |
| `updated_at` | TEXT | NULL |

Seed: Caesar Salad (Vegetarian, €8.50).

### `arrangement`
| Column | Type | Constraints |
|---|---|---|
| `id` | INTEGER | PK, AUTOINCREMENT |
| `name` | TEXT | NOT NULL |
| `base_price` | TEXT | NOT NULL |
| `type` | TEXT | NOT NULL, CHECK IN ('TwoCourse', 'ThreeCourse', 'FourCourse', 'Wine') |
| `created_at` | TEXT | NOT NULL |
| `updated_at` | TEXT | NULL |

Seed: Classic Dinner (TwoCourse, €29.99). Note: `base_price` is TEXT — likely should be REAL for arithmetic.

### `reservation`
| Column | Type | Constraints |
|---|---|---|
| `id` | INTEGER | PK, AUTOINCREMENT |
| `user_id` | INTEGER | NOT NULL, FK → `user.id` |
| `table_id` | INTEGER | NOT NULL, FK → `table.id` |
| `start_time` | TEXT | NOT NULL |
| `end_time` | TEXT | NOT NULL |
| `guests` | INTEGER | NOT NULL |
| `status` | TEXT | NOT NULL, CHECK IN ('Pending', 'Confirmed', 'Ongoing', 'Cancelled', 'Completed') |
| `created_at` | TEXT | NOT NULL |
| `updated_at` | TEXT | NULL |

Seed: One confirmed reservation on 2026-03-24 19:00–21:00, 2 guests.

### `arrangement_dish` (junction)
| Column | Type | Constraints |
|---|---|---|
| `id` | INTEGER | PK, AUTOINCREMENT |
| `dish` | INTEGER | NOT NULL, FK → `dish.id` |
| `arrangement` | INTEGER | NOT NULL, FK → `arrangement.id` |

Links dishes to arrangements (M:N). Seed: Dish 1 → Arrangement 1.

### `reservation_arrangement` (junction)
| Column | Type | Constraints |
|---|---|---|
| `id` | INTEGER | PK, AUTOINCREMENT |
| `reservation` | INTEGER | NOT NULL, FK → `reservation.id` |
| `arrangement` | INTEGER | NOT NULL, FK → `arrangement.id` |

Links reservations to arrangements (M:N). Seed: Reservation 1 → Arrangement 1.

### ER Diagram (simplified)

```
user ──< reservation >── table
              │
              M
              │
         arrangement
              │
              M
              │
            dish
```

---

## 5. NuGet Packages

| Package | Version | Project | Purpose |
|---|---|---|---|
| `Microsoft.Extensions.Hosting` | 10.0.5 | DebugDiner | IHostBuilder, DI container |
| `Microsoft.Extensions.Hosting.Abstractions` | 10.0.5 | DebugDiner | IHost, IHostedService |
| `Serilog` | 4.3.1 | DebugDiner, Infrastructure | Structured logging |
| `Serilog.Sinks.File` | 7.0.0 | DebugDiner | Rolling file log sink |
| `Serilog.Extensions.Logging` | 10.0.0 | DebugDiner | Bridges Serilog into MEL |
| `Microsoft.Extensions.Options` | 10.0.5 | Application, Infrastructure | IOptions<T> pattern |
| `Microsoft.Extensions.Options.ConfigurationExtensions` | 10.0.5 | Infrastructure | `.BindConfiguration()` |
| `Microsoft.Data.Sqlite` | 10.0.5 | Application | SQLite client |
| `Microsoft.Data.Sqlite.Core` | 10.0.5 | Domain | SqliteConnection type |

---

## 6. Configuration

All environments (Development, Production) resolve to `Data Source=database.db;` — a relative path placing the database file in the working directory at runtime.

`DatabaseOptions.ResolvedSource()` supports `%APPDATA%` expansion, but it is not currently used.

`appsettings.template.json` has a typo: `"Catabase"` instead of `"Database"` — this file serves no runtime purpose but should be corrected for clarity.

---

## 7. Known Issues

| # | Severity | Location | Issue |
|---|---|---|---|
| 1 | **Critical** | `ServiceExtensions.cs` | `extension(...)` syntax is invalid C#; prevents compilation. Should be `public static ... AddInfrastructure(this IServiceCollection services)`. |
| 2 | **High** | All migration `.sql` files | `default now()` / `now()` is not valid SQLite. Must use `DEFAULT (datetime('now'))` or `DEFAULT CURRENT_TIMESTAMP`. |
| 3 | **High** | All migration `.sql` files | `DROP TABLE IF EXISTS` + seed `INSERT` on every startup means all data is wiped on each run. Needs a one-time migration strategy or conditional seeding. |
| 4 | **Medium** | `Arrangement.sql` | `base_price` column is `TEXT` — should be `REAL` for numeric operations. |
| 5 | **Medium** | `appsettings.template.json` | Typo: `"Catabase"` should be `"Database"`. |
| 6 | **Low** | `DebugDiner.Application/DataService.cs` | File is completely empty. Application layer has no implementations. |
| 7 | **Info** | `reservation` FK column names | `user_id` / `table_id` follow `_id` convention, but junction tables use bare names (`dish`, `arrangement`, `reservation`) — inconsistent naming. |

---

## 8. Architecture Assessment

The solution follows **Clean Architecture** (also called Onion Architecture) correctly in its layering — dependencies point inward toward the Domain. The Dependency Inversion Principle is applied: the Infrastructure implements `IDataService` defined in Domain, not the other way around.

**What's correct:**
- Domain is free of infrastructure concerns
- `IDataService` is the only abstraction but it's well-placed
- DI composition root is in the outermost layer (Program.cs)
- Options pattern used for config binding
- Embedded resources for migrations avoids file-path issues at runtime
- WAL mode is an appropriate SQLite choice for this use case

**What's missing / next logical steps:**
- Application layer needs use-case classes (e.g. `CreateReservationService`, `ListAvailableTablesService`)
- Repository interfaces in Domain (e.g. `IReservationRepository`) and implementations in Infrastructure
- No console UI / menu loop implemented
- No authentication/session logic despite `password_hash` existing in the schema
- Migration strategy — currently destructive on every start

---

## 9. Project Context (from `.claude/` docs)

- **Methodology:** Scrum
- **Restaurant capacity:** 8×2-person, 5×4-person, 2×6-person tables, 1 bar (8 seats)
- **Menu:** Rotates monthly; categories Meat / Fish / Vegetarian / Vegan
- **Arrangements:** 2-, 3-, 4-course dinners; optional Wine package add-on
- **Roles:** Admin, Employee, Customer
- **Reservation statuses:** Pending → Confirmed → Ongoing → Completed (or Cancelled)