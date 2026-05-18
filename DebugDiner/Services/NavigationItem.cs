namespace DebugDiner.Services;

public record NavigationItem(string Label, Action<INavigationService> Navigate);