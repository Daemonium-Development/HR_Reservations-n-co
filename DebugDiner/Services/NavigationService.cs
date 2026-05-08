using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;

namespace DebugDiner.Services;

public class NavigationService(IServiceProvider services) : INavigationService
{
    private View? _contentArea;
    private Type? _currentViewType;
    private readonly Stack<Type> _history = new();

    public void SetContentArea(View contentArea)
    {
        _contentArea = contentArea;
    }

    public void NavigateTo<TView>() where TView : View
    {
        if (_contentArea is null) return;

        if (_currentViewType is not null)
        {
            _history.Push(_currentViewType);
        }

        _currentViewType = typeof(TView);

        var view = services.GetRequiredService<TView>();

        SwapContent(view);
    }

    public void NavigateBack()
    {
        if (_contentArea is null || _history.Count == 0)
            return;

        _currentViewType = _history.Pop();

        var view = (View)services.GetRequiredService(_currentViewType);

        SwapContent(view);
    }

    private void SwapContent(View view)
    {
        if (_contentArea is null) return;

        // 🔥 SAFE SWAP: remove only active content
        var oldContent = _contentArea.Subviews.FirstOrDefault();

        if (oldContent != null)
        {
            _contentArea.Remove(oldContent);
        }

        view.X = 0;
        view.Y = 0;
        view.Width = Dim.Fill();
        view.Height = Dim.Fill();

        _contentArea.Add(view);

        // Terminal.Gui handles redraw internally
        _contentArea.SetNeedsDisplay();
    }
}