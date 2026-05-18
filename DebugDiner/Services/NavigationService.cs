using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;

namespace DebugDiner.Services;

public class NavigationService(IServiceProvider services) : INavigationService
{
    private View? _contentArea;
    private Type? _currentViewType;
    private readonly Stack<Type> _history = new();

    private static readonly HashSet<Type> _transientViews =
    [
        typeof(CreateUserView),
        typeof(UpdateUserView),
        typeof(DeleteUserView),
        typeof(CreateDishView),
        typeof(UpdateDishView),
        typeof(DeleteDishView),
        typeof(CreateReservationsView),
        typeof(UpdateReservationView),
        typeof(DeleteReservationView),
    ];

    public event Action<IEnumerable<NavigationItem>>? NavigationItemsChanged;

    public void SetContentArea(View contentArea)
    {
        _contentArea = contentArea;
    }

    public void NavigateTo<TView>() where TView : View
    {
        if (_contentArea is null)
        {
            return;
        }

        if (_currentViewType is not null && !_transientViews.Contains(_currentViewType))
        {
            _history.Push(_currentViewType);
        }

        _currentViewType = typeof(TView);
        SwapContent(services.GetRequiredService<TView>());
        RaiseNavigationItemsChanged();
    }

    public void NavigateBack()
    {
        if (_contentArea is null || _history.Count == 0)
        {
            return;
        }

        _currentViewType = _history.Pop();
        var view = (View)services.GetRequiredService(_currentViewType);
        SwapContent(view);
        RaiseNavigationItemsChanged();
    }

    public void ClearHistory() => _history.Clear();

    private void SwapContent(View view)
    {
        if (_contentArea is null)
        {
            return;
        }

        var oldContent = _contentArea.Subviews.FirstOrDefault();
        if (oldContent is not null)
        {
            _contentArea.Remove(oldContent);
        }

        view.X = 0;
        view.Y = 0;
        view.Width = Dim.Fill();
        view.Height = Dim.Fill();

        _contentArea.Add(view);
        _contentArea.SetNeedsDisplay();
    }

    private void RaiseNavigationItemsChanged()
    {
        if (_currentViewType is null)
        {
            return;
        }

        var items = NavigationRegistry.GetItemsFor(_currentViewType, this);
        NavigationItemsChanged?.Invoke(items);
    }
}