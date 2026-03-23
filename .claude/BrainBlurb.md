# DI Container Placement — Composition Root

## Answer: The DI Container Belongs in `DebugDiner` (the startup project)

The canonical pattern here is the **Composition Root** — DI wiring should happen at the entry point, which is `Program.cs` in `DebugDiner`.

### The core problem with putting it in Application

The Application layer is only supposed to depend on the Domain layer. But DI registration needs to wire concrete Infrastructure implementations (e.g., `ReservationRepository`) to Domain interfaces (e.g., `IReservationRepository`). If Application does that wiring, it suddenly needs a reference to Infrastructure — **breaking the dependency rule**.

```
Application → Domain         ✓ (current rule)
Application → Infrastructure ✗ (would be a violation)
```

### The right approach: each layer owns its registration, startup calls them all

Each layer exposes an extension method that registers only its own internals:

```csharp
// In DebugDiner.Application
public static IServiceCollection AddApplicationServices(this IServiceCollection services)
{
    services.AddScoped<IReservationService, ReservationService>();
    services.AddScoped<IAuthService, AuthService>();
    // ...
    return services;
}

// In DebugDiner.Infrastructure
public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration config)
{
    services.AddDbContext<AppDbContext>(...);
    services.AddScoped<IReservationRepository, ReservationRepository>();
    // ...
    return services;
}
```

Then `Program.cs` (the `ConfigureServices` call) becomes the Composition Root:

```csharp
.ConfigureServices((ctx, services) =>
{
    services.AddApplicationServices();
    services.AddInfrastructureServices(ctx.Configuration);
})
```

### Result: dependency arrows stay correct

```
DebugDiner (startup) → Application + Infrastructure  ✓ (only startup knows all layers)
Application          → Domain                         ✓
Infrastructure       → Domain                         ✓
Domain               → nothing                        ✓
```

**tl;dr** — Put the wiring in `Program.cs` (`DebugDiner`). Let Application and Infrastructure each expose an `AddXxxServices()` extension method. This keeps your layer boundaries clean and is standard .NET practice.
