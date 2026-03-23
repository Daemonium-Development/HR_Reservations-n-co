```mermaid
---
title: Main Architecture
---
graph TB
    subgraph Presentation["🖥️ Presentation Layer"]
        CLI["Console UI\n(Program.cs / ConsoleRunner)"]
        LOGIN["Login / Register Screen"]
        subgraph CustomerFlow["Customer Flow"]
            CM["Customer Menu Handler"]
            CF["Make / View / Cancel\nOwn Reservations"]
        end
        subgraph AdminFlow["Admin Flow (requires Admin role)"]
            AM["Admin Menu Handler"]
            AF1["Manage All Bookings\n(View / Edit / Cancel)"]
            AF2["Manage Monthly Menu\n(Dishes & Arrangements)"]
            AF3["Manage Users"]
            AF4["View Occupancy / Reports"]
        end
        Formatters["Display Formatters\n(Tables, Receipts, Availability)"]
    end

    subgraph Application["⚙️ Application Layer"]
        AUTH["AuthService\n(Login, Register, Role check)"]
        RS["ReservationService"]
        TS["TableService"]
        MS["MenuService"]
        AS["ArrangementService"]
        AV["AvailabilityService"]
        US["UserService"]
    end

    subgraph Domain["📦 Domain Layer"]
        subgraph Entities["Entities"]
            USER["User\n(Id, Name, Email,\nPasswordHash, Role)"]
            RES["Reservation\n(linked to UserId)"]
            TABLE["Table"]
            MENU["Menu"]
            DISH["Dish"]
            ARR["Arrangement"]
        end
        subgraph ValueObjects["Value Objects"]
            TS_VO["TimeSlot"]
            ROLE["Role\n(Customer / Admin)"]
            ARR_TYPE["ArrangementType\n(2 / 3 / 4 course + wine)"]
            TABLE_TYPE["TableType\n(2p / 4p / 6p / Bar)"]
            DISH_CAT["DishCategory\n(Meat / Fish / Veg / Vegan)"]
        end
        subgraph Interfaces["Repository Interfaces"]
            IR["IReservationRepository"]
            IT["ITableRepository"]
            IM["IMenuRepository"]
            IU["IUserRepository"]
        end
    end

    subgraph Infrastructure["🗄️ Infrastructure Layer"]
        subgraph Persistence["Data Persistence"]
            SQLITE["SQLite Database\n(restaurant.db)"]
            EF["EF Core\n(Microsoft.EntityFrameworkCore.Sqlite)"]
            REPO_R["ReservationRepository"]
            REPO_T["TableRepository"]
            REPO_M["MenuRepository"]
            REPO_U["UserRepository"]
        end
        subgraph Seed["Data Seeding"]
            SEED["DataSeeder\n(Tables, Initial Menu,\nDefault Admin account)"]
            MIG["EF Migrations"]
        end
    end

    subgraph CrossCutting["🔧 Cross-Cutting Concerns"]
        VALID["Validation\n(FluentValidation)"]
        ERR["Error Handling\n(Result<T> pattern)"]
        LOG["Logging\n(Console / File)"]
        DI["DI Container\n(Microsoft.Extensions.DI)"]
        SESSION["Session Context\n(CurrentUser)"]
    end

    CLI --> LOGIN
    LOGIN --> AUTH
    AUTH -->|Customer role| CM
    AUTH -->|Admin role| AM
    CM --> CF
    AM --> AF1
    AM --> AF2
    AM --> AF3
    AM --> AF4
    CF --> RS
    CF --> AV
    AF1 --> RS
    AF2 --> MS
    AF2 --> AS
    AF3 --> US
    AF4 --> RS
    AF4 --> TS
    CM --> Formatters
    AM --> Formatters

    AUTH --> IU
    RS --> IR
    TS --> IT
    MS --> IM
    AS --> IM
    AV --> IR
    AV --> IT
    US --> IU
    RS --> VALID
    RS --> ERR
    AUTH --> SESSION

    IR --> REPO_R
    IT --> REPO_T
    IM --> REPO_M
    IU --> REPO_U

    REPO_R --> EF
    REPO_T --> EF
    REPO_M --> EF
    REPO_U --> EF

    EF --> SQLITE
    MIG --> SQLITE
    SEED --> EF
    DI -.->|wires| Application
    DI -.->|wires| Infrastructure
    SESSION -.->|injected into| Application
```

The Presentation layer depends only on the application layer, which allows for the application-layer to handle all the interactions and pass through what needs to happen to the presentation layer.

The Application-Layer only depends on the Domain layer, allowing orchestration to happen between application and the services.

The Domain layer has no dependencies on its own, allowing others to depend on the domain

The Infrastructure layer depends on the interfaces from the Domain layer, implementing the methods. This creates a barrier between the services and the presentation.