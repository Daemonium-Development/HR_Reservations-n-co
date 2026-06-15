using Microsoft.Extensions.DependencyInjection;

using Terminal.Gui;

namespace DD.Presentation.Test;

// Minimal Terminal.Gui views used to drive NavigationService without pulling
// in the real UI stack. They are not in NavigationRegistry, so navigating to
// them produces an empty sidebar item set.
public sealed class StubViewA : View;

public sealed class StubViewB : View;

public sealed class StubViewC : View;

internal static class StubProvider
{
    public static IServiceProvider Build() =>
        new ServiceCollection()
            .AddTransient<StubViewA>()
            .AddTransient<StubViewB>()
            .AddTransient<StubViewC>()
            .BuildServiceProvider();
}
