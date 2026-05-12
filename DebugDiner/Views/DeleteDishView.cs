using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class DeleteDishView : BaseView
{
    public DeleteDishView(INavigationService nav, IMenuRepository menuRepository) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Admin Panel");
        SetContentTitle("Delete Dish");

        var selectedDish = AppState.SelectedDish;

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var questionLabel = new Label
        {
            X = Pos.Center(),
            Y = 5,
            Text = "Are you sure you want to delete",
        };

        var nameLabel = new Label
        {
            X = Pos.Center(),
            Y = questionLabel.Y + 1,
            Text = selectedDish.Name,
        };

        var yesBtn = new Button
        {
            X = 5,
            Y = nameLabel.Y + 3,
            AutoSize = true,
            Text = $"Yes, delete {selectedDish.Name}",
        };

        yesBtn.Clicked += () =>
        {
            try
            {
                menuRepository.Delete([selectedDish]).GetAwaiter().GetResult();
                nav.NavigateTo<DishView>();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to delete dish: {ex.Message}", "OK");
            }
        };

        var noBtn = new Button
        {
            X = Pos.Right(yesBtn) + 5,
            Y = yesBtn.Y,
            AutoSize = true,
            Text = "No, go back",
        };

        noBtn.Clicked += () => nav.NavigateTo<DishView>();

        container.Add(questionLabel, nameLabel, yesBtn, noBtn);
        SetContent(container);
    }
}
