using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

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
            nav.NavigateTo<AdminUsersView>();
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
        nameLabel.GetCurrentHeight(out var nameLabelHeight);

        var nameInput = new TextField
        {
            X = 0,
            Y = nameLabelHeight + nameLabel.Y,
            Width = Dim.Fill(),
            Text = user.Name,
        };
        nameInput.GetCurrentHeight(out var nameInputHeight);

        var emailLabel = new Label
        {
            X = 0,
            Y = (nameInputHeight + nameInput.Y) + 1,
            Text = "Email:",
        };
        emailLabel.GetCurrentHeight(out var emailLabelHeight);

        var emailInput = new TextField
        {
            X = 0,
            Y = emailLabelHeight + emailLabel.Y,
            Width = Dim.Fill(),
            Text = user.Email,
        };
        emailInput.GetCurrentHeight(out var emailInputHeight);

        var roleLabel = new Label
        {
            X = 0,
            Y = (emailInputHeight + emailInput.Y) + 1,
            Text = "Role:",
        };
        roleLabel.GetCurrentHeight(out var roleLabelHeight);

        var roleNames = Enum.GetNames<Role>();
        var roleGroup = new RadioGroup(roleNames.Select(n => (NStack.ustring)n).ToArray())
        {
            X = 0,
            Y = roleLabelHeight + roleLabel.Y,
            SelectedItem = Array.IndexOf(roleNames, user.Role.ToString()),
        };
        roleGroup.GetCurrentHeight(out var roleGroupHeight);

        var submitBtn = new Button
        {
            X = 0,
            Y = (roleGroupHeight + roleGroup.Y) + 1,
            AutoSize = true,
            Text = "Update User",
        };

        submitBtn.Clicked += () =>
        {
            if (!Enum.TryParse<Role>(roleNames[roleGroup.SelectedItem], out var newRole)) return;

            user.Name      = nameInput.Text.ToString()  ?? user.Name;
            user.Email     = emailInput.Text.ToString() ?? user.Email;
            user.Role      = newRole;
            user.UpdatedAt = DateTime.UtcNow;

            try
            {
                userRepository.Update([user]).GetAwaiter().GetResult();
                nav.NavigateTo<AdminUsersView>();
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
