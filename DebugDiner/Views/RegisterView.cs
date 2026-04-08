using DebugDiner.Application;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class RegisterView : BaseView
{
    public RegisterView(INavigationService nav, IAuthService auth) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Register");
        SetContentTitle("Register");

        var container = new View
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
            X = 5,
            Y = 4,
            Width = 30,
        };

        var emailLabel = new Label
        {
            X = 5,
            Y = 6,
            Text = "Email:",
        };

        var emailField = new TextField
        {
            X = 5,
            Y = 7,
            Width = 30,
        };

        var passwordLabel = new Label
        {
            X = 5,
            Y = 9,
            Text = "Password:",
        };

        var passwordField = new TextField
        {
            X = 5,
            Y = 10,
            Width = 30,
            Secret = true,
        };

        var errorLabel = new Label
        {
            X = 5,
            Y = 12,
            Text = string.Empty,
        };

        var registerBtn = new Button
        {
            X = 5,
            Y = 14,
            Text = "Register",
            IsDefault = true,
        };
        registerBtn.Clicked += () =>
        {
            var name = nameField.Text.ToString() ?? string.Empty;
            var email = emailField.Text.ToString() ?? string.Empty;
            var password = passwordField.Text.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                errorLabel.Text = "All fields are required.";
                return;
            }

            var result = auth.RegisterAsync(name, email, password).GetAwaiter().GetResult();
            if (result is null)
            {
                errorLabel.Text = "Registration failed. Email may already be in use.";
                return;
            }

            nav.NavigateTo<LoginView>();
        };

        container.Add(nameLabel, nameField, emailLabel, emailField, passwordLabel, passwordField, errorLabel, registerBtn);
        SetContent(container);
    }
}