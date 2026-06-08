# Object-Oriented Design Patterns & Concepts — Debug Diner

## Checklist

| Concept | Used | Location |
|---|---|---|
| Classes & Objects | ✅ | All layers |
| Encapsulation | ✅ | All repositories, BaseEntity |
| Inheritance | ✅ | BaseEntity → all entities; BaseView → all views |
| Interfaces | ✅ | IUserRepository, IMenuRepository, IReservationRepository, ITableRepository, IArrangementRepository, IDataService, IAuthService, INavigationService |
| Generics | ✅ | IEnumerable<T>, Task<T> throughout all repositories |
| Enumerations | ✅ | Role, TableType, ArrangementType, DishCategory, ReservationStatus |
| Properties | ✅ | All entities (get; set;) |
| Dependency Injection | ✅ | Program.cs, ServiceExtensions.cs |
| Polymorphism | ✅ | BaseView subclasses, Repository implementations via interfaces |
| Composition | ✅ | MenuRepository depends on IArrangementRepository |
| Async/Await | ✅ | All repository methods return Task<T> |

---

## 1. Encapsulation

**Where:** `DebugDiner.Domain/Entities/BaseEntity.cs` (lines 13–19) and all entity classes

**Why:** Properties expose data through controlled `get; set;` access rather than public fields. This ensures EF Core can track changes and prevents invalid state.

**Why not public fields:** Public fields bypass validation, cannot be used as EF Core column mappings, and break encapsulation contracts.

```csharp
// BaseEntity.cs — lines 13-19
public required int Id { get; set; }
[Column("created_at")]
public required DateTime CreatedAt { get; set; }
[Column("updated_at")]
public DateTime? UpdatedAt { get; set; }
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Domain/Entities/BaseEntity.cs#L13

---

## 2. Inheritance

**Where:** `DebugDiner.Domain/Entities/` — all entities extend `BaseEntity`. `DebugDiner/Views/` — all views extend `BaseView`.

**Why:** `BaseEntity` centralises `Id`, `CreatedAt`, `UpdatedAt`, `Equals()`, and `GetHashCode()` — avoiding code duplication across 5 entity classes. `BaseView` centralises the Terminal.GUI layout (header, nav panel, content frame) shared by all screens.

**Why not interfaces alone:** Interfaces define contracts but cannot provide shared implementation. Inheritance is appropriate here because entities share both structure AND behaviour (equality comparison).

```csharp
// ReservationEntity.cs — line 4
public class ReservationEntity : BaseEntity

// UserRepository.cs — line 8
public class UserRepository(ILogger logger, IDataService data) : IUserRepository

// CreateReservationsView.cs — line 7
public class CreateReservationsView : BaseView
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Domain/Entities/ReservationEntity.cs#L4

---

## 3. Interfaces

**Where:** `DebugDiner.Domain/Abstractions/` — IUserRepository, IMenuRepository, IReservationRepository, ITableRepository, IArrangementRepository, IDataService. `DebugDiner.Application/Abstractions/` — IAuthService. `DebugDiner/Services/` — INavigationService.

**Why:** Interfaces decouple the Application layer from Infrastructure. Services depend on `IUserRepository`, not `UserRepository`. This means the database can be swapped without touching application logic (Dependency Inversion Principle).

**Why not abstract class:** Repositories share no common implementation — only a contract. An abstract class would force artificial coupling between unrelated repositories.

```csharp
// IUserRepository.cs
public interface IUserRepository
{
    Task<IEnumerable<UserEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
    Task<IEnumerable<UserEntity>> Create(IEnumerable<UserEntity> users);
    Task<IEnumerable<UserEntity>> Update(IEnumerable<UserEntity> users);
    Task<int> Delete(IEnumerable<UserEntity> users);
}
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Domain/Abstractions/IUserRepository.cs

---

## 4. Generics

**Where:** All repository interfaces and implementations — `IEnumerable<T>`, `Task<T>` (e.g. `Task<IEnumerable<UserEntity>>`).

**Why:** Generic collections allow type-safe batch operations (create/update/delete multiple items) without boxing overhead or casts. `Task<T>` enables async return values with a known type.

**Why not `object` or non-generic collections:** Would require casting, lose compile-time type safety, and reduce readability.

```csharp
// IMenuRepository.cs — line 5
Task<IEnumerable<DishEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
Task<IEnumerable<DishEntity>> Create(IEnumerable<DishEntity> dishes);
Task<IEnumerable<DishEntity>> Update(IEnumerable<DishEntity> dishes);
Task<int> Delete(IEnumerable<DishEntity> dishes);
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Domain/Abstractions/IMenuRepository.cs

---

## 5. Enumerations

**Where:** `DebugDiner.Domain/Entities/Enums.cs` — Role, TableType, ArrangementType, DishCategory, ReservationStatus.

**Why:** Enums restrict values to a known set, preventing invalid states like `role = "superadmin"`. They are stored as integers in SQLite (space-efficient) and readable in code.

**Why not string constants:** Strings are error-prone (typos), not type-safe, and make switch/match exhaustiveness checking impossible.

```csharp
// Enums.cs
public enum Role       { Admin, Customer, Employee, Manager }
public enum TableType  { TwoPerson, FourPerson, SixPerson, Bar }
public enum ReservationStatus { Pending, Confirmed, OnGoing, Completed, Cancelled }
public enum ArrangementType   { TwoCourse, ThreeCourse, FourCourse, Wine }
public enum DishCategory      { Meat, Fish, Vegetarian, Vegan }
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Domain/Entities/Enums.cs

---

## 6. Dependency Injection

**Where:** `DebugDiner.Infrastructure/ServiceExtensions.cs` (lines 18–24) and `DebugDiner/Program.cs` (lines 51–67).

**Why:** DI wires interfaces to implementations at startup without hard-coding `new UserRepository()` throughout the codebase. This makes testing easy (inject mocks) and satisfies the Dependency Inversion Principle.

**Why not `new` keyword:** Manual instantiation creates tight coupling, makes testing impossible without real databases, and requires changing every call site to swap implementations.

```csharp
// ServiceExtensions.cs — lines 18-24
services.AddSingleton<IDataService, DataService>();
services.AddSingleton<IArrangementRepository, ArrangementRepository>();
services.AddSingleton<IMenuRepository, MenuRepository>();
services.AddSingleton<IReservationRepository, ReservationRepository>();
services.AddSingleton<ITableRepository, TableRepository>();
services.AddSingleton<IUserRepository, UserRepository>();
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Infrastructure/ServiceExtensions.cs#L18

---

## 7. Polymorphism

**Where:** All repository classes implement their interface — `UserRepository : IUserRepository`, etc. All view classes extend `BaseView`.

**Why:** Application services accept `IUserRepository` (the interface type). The actual implementation (`UserRepository`) is resolved at runtime by DI. This is runtime polymorphism: the same method call behaves differently based on the actual object.

```csharp
// Runtime polymorphism — the service doesn't know which implementation it receives
public class AuthService(IUserRepository userRepository, ILogger logger) : IAuthService
{
    // userRepository could be UserRepository or a mock in tests
}
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Application/Services/AuthService.cs

---

## 8. Composition

**Where:** `MenuRepository` composes `IArrangementRepository` — `MenuRepository(ILogger logger, IDataService data, IArrangementRepository arrangementRepository)`.

**Why:** Dishes can have associated arrangements. Rather than duplicating arrangement logic, `MenuRepository` delegates to the `IArrangementRepository`. This is composition over inheritance — MenuRepository "has-a" ArrangementRepository.

```csharp
// MenuRepository.cs — line 8
public class MenuRepository(ILogger logger, IDataService data, IArrangementRepository arrangementRepository)
    : IMenuRepository
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Infrastructure/Repositories/MenuRepository.cs#L8

---

## 9. Async/Await

**Where:** All repository methods — `GetItemsAsync`, `Create`, `Update`, `Delete`. DataService — `StartAsync`, `StopAsync`, `RestartAsync`.

**Why:** Database I/O is inherently blocking. Async/await frees the thread while waiting for SQLite, keeping the Terminal.GUI application responsive.

**Why not synchronous:** Synchronous DB calls block the UI thread, causing the console application to freeze during queries.

```csharp
// UserRepository.cs
public async Task<IEnumerable<UserEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
{
    var reader = await command.ExecuteReaderAsync();
    // ...
}
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Infrastructure/Repositories/UserRepository.cs#L10
