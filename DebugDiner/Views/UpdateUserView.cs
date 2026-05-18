using Terminal.Gui;
using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;

namespace DebugDiner;

public class UpdateUserView : BaseView
{
    public UpdateUserView(INavigationService nav, IUserRepository userRepository) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Admin Panel");
        SetContentTitle("Update User");

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

        var nameLabel = new Label
        {
            X = 0,
            Y = 0,
            Text = "Name:",
        };

        var nameInput = new TextField
        {
            X = 0,
            Y = 1,
            Width = Dim.Fill(),
            Text = user.Name,
        };

        var emailLabel = new Label
        {
            X = 0,
            Y = 3,
            Text = "Email:",
        };

        var emailInput = new TextField
        {
            X = 0,
            Y = 4,
            Width = Dim.Fill(),
            Text = user.Email,
        };

        var roleLabel = new Label
        {
            X = 0,
            Y = 6,
            Text = "Role:",
        };

        var roleNames = Enum.GetNames<Role>();
        var roleGroup = new RadioGroup(roleNames.Select(n => (NStack.ustring)n).ToArray())
        {
            X = 0,
            Y = 7,
            SelectedItem = Array.IndexOf(roleNames, user.Role.ToString()),
        };

        var submitBtn = new Button
        {
            X = 0,
            Y = 12,
            AutoSize = true,
            Text = "Update User",
        };

        submitBtn.Clicked += () =>
        {
            if (!Enum.TryParse<Role>(roleNames[roleGroup.SelectedItem], out var newRole))
            {
                return;
            }

            user.Name      = nameInput.Text.ToString()  ?? user.Name;
            user.Email     = emailInput.Text.ToString() ?? user.Email;
            user.Role      = newRole;
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                userRepository.Update([user]).GetAwaiter().GetResult();
                AppState.SelectedUser = null;
                nav.NavigateBack();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to update user: {ex.Message}", "OK");
            }
        };

        container.Add(nameLabel, nameInput, emailLabel, emailInput, roleLabel, roleGroup, submitBtn);
        SetContent(container);
    }
}
