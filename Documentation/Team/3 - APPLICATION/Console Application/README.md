# Debug Diner — Console Application

## Requirements

- .NET 10.0 SDK
- Windows, macOS, or Linux

## How to run

```bash
git clone https://github.com/Daemonium-Development/HR_Reservations-n-co.git
cd HR_Reservations-n-co
dotnet run --project DebugDiner
```

## Solution structure

| Project | Description |
|---|---|
| `DebugDiner` | Presentation layer — Terminal.GUI views, navigation |
| `DebugDiner.Application` | Application layer — services, auth |
| `DebugDiner.Domain` | Domain layer — entities, enums, interfaces |
| `DebugDiner.Infrastructure` | Infrastructure layer — SQLite repositories, EF Core |
| `DD.Infra.Test` | xUnit test project |

The application uses a `.slnx` solution file (`ReservationsCo.slnx`).
The SQLite database is created automatically on first run via EF Migrations and DataSeeder.

## Default admin account (seeded)

See `DataSeeder.cs` in `DebugDiner.Infrastructure` for seeded credentials.
