using Terminal.Gui;

namespace DebugDiner.Services;

public interface INavigationService
{
    void NavigateTo<TView>() where TView : View;
    void NavigateBack();
    void SetContentArea(View contentArea);
}