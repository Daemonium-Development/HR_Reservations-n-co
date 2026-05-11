using Terminal.Gui;

namespace DebugDiner.Services;

public interface INavigationService
{
    public event Action<IEnumerable<string>>? NavigationItemsChanged;

    public void NavigateTo<TView>() where TView : View;
    public void SetContentArea(View contentArea);
    public void NavigateBack();
}
