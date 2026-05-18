using Terminal.Gui;
using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;

namespace DebugDiner;

public class DeleteUserView : BaseView
{
    public DeleteUserView(INavigationService nav, IUserRepository userRepository) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Admin Panel");
        SetContentTitle("Delete User");

        var user = AppState.SelectedUser;

        if (user is null)
        {
            nav.NavigateBack();
            return;
        }

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
            Text = user.Name,
        };

        var yesBtn = new Button
        {
            X = 5,
            Y = nameLabel.Y + 3,
            AutoSize = true,
            Text = "Yes, delete user",
        };

        yesBtn.Clicked += () =>
        {
            try
            {
                userRepository.Delete([user]).GetAwaiter().GetResult();
                AppState.SelectedUser = null;
                nav.NavigateBack();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to delete user: {ex.Message}", "OK");
            }
        };

        var noBtn = new Button
        {
            X = Pos.Right(yesBtn) + 5,
            Y = yesBtn.Y,
            AutoSize = true,
            Text = "No, go back",
        };

        noBtn.Clicked += nav.NavigateBack;

        container.Add(questionLabel, nameLabel, yesBtn, noBtn);
        SetContent(container);
    }
}
