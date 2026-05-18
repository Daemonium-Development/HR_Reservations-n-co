using DebugDiner.Application;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class CreateUserView : BaseView
{
    public CreateUserView(INavigationService nav, IAuthService auth) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Add User");

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var frame = new FrameView("Create User")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var nameLabel = new Label
        {
            X = 5,
            Y = 3,
            Text = "Name:",
        };

        var nameField = new TextField
        {
            X = 20,
            Y = 3,
            Width = 30,
        };

        var emailLabel = new Label
        {
            X = 5,
            Y = 5,
            Text = "Email:",
        };

        var emailField = new TextField
        {
            X = 20,
            Y = 5,
            Width = 40,
        };

        var passwordLabel = new Label
        {
            X = 5,
            Y = 7,
            Text = "Password:",
        };

        var passwordField = new TextField
        {
            X = 20,
            Y = 7,
            Width = 30,
            Secret = true,
        };

        var isAdminCheckBox = new CheckBox
        {
            X = 5,
            Y = 9,
            Text = "Grant admin role",
            Checked = false,
        };

        var errorLabel = new Label
        {
            X = 5,
            Y = 11,
            Text = string.Empty,
        };

        var createBtn = new Button
        {
            X = 5,
            Y = 13,
            Text = "Create",
        };

        createBtn.Clicked += () =>
        {
            var name = nameField.Text.ToString() ?? string.Empty;
            var email = emailField.Text.ToString() ?? string.Empty;
            var password = passwordField.Text.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                errorLabel.Text = "Name, email and password are required.";
                return;
            }

            var result = auth.RegisterAsync(name, email, password, isAdminCheckBox.Checked).GetAwaiter().GetResult();
            if (result is null)
            {
                errorLabel.Text = "Failed to create user. Email may already be in use.";
                return;
            }

            MessageBox.Query("Success", $"User '{result.Name}' created.", "OK");
            nav.NavigateBack();
        };

        var cancelBtn = new Button
        {
            X = 15,
            Y = 13,
            Text = "Cancel",
        };

        cancelBtn.Clicked += nav.NavigateBack;

        frame.Add(
            nameLabel, nameField,
            emailLabel, emailField,
            passwordLabel, passwordField,
            isAdminCheckBox,
            errorLabel,
            createBtn, cancelBtn
        );

        container.Add(frame);
        SetContent(container);
    }
}
