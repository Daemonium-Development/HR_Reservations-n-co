using Microsoft.Extensions.DependencyInjection;
using Terminal.Gui;

namespace DebugDiner.Services;

public class NavigationService(IServiceProvider services) : INavigationService
{
    private View? _contentArea;
    public void SetContentArea(View contentArea) => _contentArea = contentArea;

    public void NavigateTo<TView>()
        where TView : View
    {
        if (_contentArea is null)
        {
            return;
        }

        foreach (var sub in _contentArea.Subviews.ToList())
        {
            _contentArea.Remove(sub);
        }

        var view = services.GetRequiredService<TView>();
        view.X = 0;
        view.Y = 0;
        view.Width = Dim.Fill();
        view.Height = Dim.Fill();

        _contentArea.Add(view);
        _contentArea.SetNeedsDraw();
    }
}
