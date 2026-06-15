# Objectgeoriënteerde ontwerppatronen en -concepten - Debug Diner

## Checklist

| Concept | Gebruikt | Locatie |
|---|---|---|
| Classes & Objects | ✅ | Alle lagen |
| Encapsulation | ✅ | Alle repositories, BaseEntity |
| Inheritance | ✅ | BaseEntity → alle entiteiten; BaseView → alle views |
| Interfaces | ✅ | IUserRepository, IMenuRepository, IReservationRepository, ITableRepository, IArrangementRepository, IDataService, IAuthService, INavigationService |
| Generics | ✅ | IEnumerable<T>, Task<T> in alle repositories |
| Enumerations | ✅ | Role, TableType, ArrangementType, DishCategory, ReservationStatus |
| Properties | ✅ | Alle entiteiten (get; set;) |
| Dependency Injection | ✅ | Program.cs, ServiceExtensions.cs |
| Polymorphism | ✅ | BaseView-subklassen, repository-implementaties via interfaces |
| Composition | ✅ | MenuRepository hangt af van IArrangementRepository |
| Async/Await | ✅ | Alle repository-methoden retourneren Task<T> |

---

## 1. Encapsulation

**Waar:** `DebugDiner.Domain/Entities/BaseEntity.cs` (regels 13-19) en alle entiteitsklassen

**Waarom:** Properties stellen data beschikbaar via gecontroleerde `get; set;`-toegang in plaats van publieke velden. Hierdoor kan EF Core wijzigingen volgen en wordt een ongeldige status voorkomen.

**Waarom geen publieke velden:** Publieke velden omzeilen validatie, kunnen niet als EF Core-kolomtoewijzingen worden gebruikt en doorbreken de afspraken rond encapsulation.

```csharp
// BaseEntity.cs - lines 13-19
public required int Id { get; set; }
[Column("created_at")]
public required DateTime CreatedAt { get; set; }
[Column("updated_at")]
public DateTime? UpdatedAt { get; set; }
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Domain/Entities/BaseEntity.cs#L13

---

## 2. Inheritance

**Waar:** `DebugDiner.Domain/Entities/` - alle entiteiten breiden `BaseEntity` uit. `DebugDiner/Views/` - alle views breiden `BaseView` uit.

**Waarom:** `BaseEntity` centraliseert `Id`, `CreatedAt`, `UpdatedAt`, `Equals()` en `GetHashCode()`, wat codeduplicatie over 5 entiteitsklassen voorkomt. `BaseView` centraliseert de Terminal.GUI-layout (header, nav-paneel, content frame) die door alle schermen wordt gedeeld.

**Waarom niet alleen interfaces:** Interfaces definiëren afspraken maar kunnen geen gedeelde implementatie bieden. Overerving is hier passend omdat entiteiten zowel structuur ALS gedrag delen (gelijkheidsvergelijking).

```csharp
// ReservationEntity.cs - line 4
public class ReservationEntity : BaseEntity

// UserRepository.cs - line 8
public class UserRepository(ILogger logger, IDataService data) : IUserRepository

// CreateReservationsView.cs - line 7
public class CreateReservationsView : BaseView
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Domain/Entities/ReservationEntity.cs#L4

---

## 3. Interfaces

**Waar:** `DebugDiner.Domain/Abstractions/` - IUserRepository, IMenuRepository, IReservationRepository, ITableRepository, IArrangementRepository, IDataService. `DebugDiner.Application/Abstractions/` - IAuthService. `DebugDiner/Services/` - INavigationService.

**Waarom:** Interfaces ontkoppelen de Application-laag van Infrastructure. Services hangen af van `IUserRepository`, niet van `UserRepository`. Hierdoor kan de database worden vervangen zonder de applicatielogica aan te raken (Dependency Inversion Principle).

**Waarom geen abstracte klasse:** Repositories delen geen gemeenschappelijke implementatie, alleen een afspraak. Een abstracte klasse zou kunstmatige koppeling tussen ongerelateerde repositories afdwingen.

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

**Waar:** Alle repository-interfaces en -implementaties - `IEnumerable<T>`, `Task<T>` (bijv. `Task<IEnumerable<UserEntity>>`).

**Waarom:** Generieke collecties maken type-veilige batchbewerkingen mogelijk (meerdere items aanmaken/bijwerken/verwijderen) zonder boxing-overhead of casts. `Task<T>` maakt asynchrone retourwaarden met een bekend type mogelijk.

**Waarom geen `object` of niet-generieke collecties:** Dat zou casten vereisen, type-veiligheid tijdens compileren verliezen en de leesbaarheid verminderen.

```csharp
// IMenuRepository.cs - line 5
Task<IEnumerable<DishEntity>> GetItemsAsync(IEnumerable<int>? ids = null);
Task<IEnumerable<DishEntity>> Create(IEnumerable<DishEntity> dishes);
Task<IEnumerable<DishEntity>> Update(IEnumerable<DishEntity> dishes);
Task<int> Delete(IEnumerable<DishEntity> dishes);
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Domain/Abstractions/IMenuRepository.cs

---

## 5. Enumerations

**Waar:** `DebugDiner.Domain/Entities/Enums.cs` - Role, TableType, ArrangementType, DishCategory, ReservationStatus.

**Waarom:** Enums beperken waarden tot een bekende set en voorkomen ongeldige statussen zoals `role = "superadmin"`. Ze worden als gehele getallen opgeslagen in SQLite (ruimtebesparend) en zijn leesbaar in code.

**Waarom geen stringconstanten:** Strings zijn foutgevoelig (typefouten), niet type-veilig en maken volledigheidscontrole bij switch/match onmogelijk.

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

**Waar:** `DebugDiner.Infrastructure/ServiceExtensions.cs` (regels 18-24) en `DebugDiner/Program.cs` (regels 51-67).

**Waarom:** DI koppelt interfaces aan implementaties bij het opstarten zonder `new UserRepository()` overal in de codebase vast te leggen. Dit maakt testen eenvoudig (mocks injecteren) en voldoet aan het Dependency Inversion Principle.

**Waarom geen `new`-keyword:** Handmatige instantiatie zorgt voor sterke koppeling, maakt testen zonder echte databases onmogelijk en vereist dat elke aanroeplocatie wordt aangepast om implementaties te wisselen.

```csharp
// ServiceExtensions.cs - lines 18-24
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

**Waar:** Alle repository-klassen implementeren hun interface - `UserRepository : IUserRepository`, enz. Alle view-klassen breiden `BaseView` uit.

**Waarom:** Application-services accepteren `IUserRepository` (het interfacetype). De daadwerkelijke implementatie (`UserRepository`) wordt tijdens runtime door DI opgelost. Dit is runtime-polymorfisme: dezelfde methodeaanroep gedraagt zich anders afhankelijk van het daadwerkelijke object.

```csharp
// Runtime polymorphism - the service doesn't know which implementation it receives
public class AuthService(IUserRepository userRepository, ILogger logger) : IAuthService
{
    // userRepository could be UserRepository or a mock in tests
}
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Application/Services/AuthService.cs

---

## 8. Composition

**Waar:** `MenuRepository` componeert `IArrangementRepository` - `MenuRepository(ILogger logger, IDataService data, IArrangementRepository arrangementRepository)`.

**Waarom:** Gerechten kunnen bijbehorende arrangementen hebben. In plaats van arrangementlogica te dupliceren, delegeert `MenuRepository` naar de `IArrangementRepository`. Dit is compositie boven overerving - MenuRepository "heeft-een" ArrangementRepository.

```csharp
// MenuRepository.cs - line 8
public class MenuRepository(ILogger logger, IDataService data, IArrangementRepository arrangementRepository)
    : IMenuRepository
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Infrastructure/Repositories/MenuRepository.cs#L8

---

## 9. Async/Await

**Waar:** Alle repository-methoden - `GetItemsAsync`, `Create`, `Update`, `Delete`. DataService - `StartAsync`, `StopAsync`, `RestartAsync`.

**Waarom:** Database-I/O is inherent blokkerend. Async/await maakt de thread vrij tijdens het wachten op SQLite, waardoor de Terminal.GUI-applicatie responsief blijft.

**Waarom niet synchroon:** Synchrone DB-aanroepen blokkeren de UI-thread, waardoor de console-applicatie tijdens query's bevriest.

```csharp
// UserRepository.cs
public async Task<IEnumerable<UserEntity>> GetItemsAsync(IEnumerable<int>? ids = null)
{
    var reader = await command.ExecuteReaderAsync();
    // ...
}
```

GitHub: https://github.com/Daemonium-Development/HR_Reservations-n-co/blob/main/DebugDiner.Infrastructure/Repositories/UserRepository.cs#L10
