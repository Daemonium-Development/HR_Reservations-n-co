using Terminal.Gui;

namespace DebugDiner.Services;

public interface INavigationService
{
    public void NavigateTo<TView>() where TView : View;
    public void SetContentArea(View contentArea);
}
