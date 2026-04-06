using DebugDiner.Application;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class LoginView : BaseView
{
    public LoginView(INavigationService nav, IAuthService auth) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Login");
        SetContentTitle("Login");

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var emailLabel = new Label
        {
            X = 5,
            Y = 3,
            Text = "Email:",
        };

        var emailField = new TextField
        {
            X = 5,
            Y = 4,
            Width = 30,
        };

        var passwordLabel = new Label
        {
            X = 5,
            Y = 6,
            Text = "Password:",
        };

        var passwordField = new TextField
        {
            X = 5,
            Y = 7,
            Width = 30,
            Secret = true,
        };

        var errorLabel = new Label
        {
            X = 5,
            Y = 9,
            Text = string.Empty,
        };

        var loginBtn = new Button
        {
            X = 5,
            Y = 11,
            Text = "Login",
            IsDefault = true,
        };
        loginBtn.Clicked += () =>
        {
            var email = emailField.Text.ToString() ?? string.Empty;
            var password = passwordField.Text.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                errorLabel.Text = "Email and password are required.";
                return;
            }

            var result = auth.LoginAsync(email, password).GetAwaiter().GetResult();
            if (result is null)
            {
                errorLabel.Text = "Invalid email or password.";
                return;
            }

            AppState.CurrentUser = result;
            nav.NavigateTo<HomeView>();
        };

        var registerBtn = new Button
        {
            X = 5,
            Y = 13,
            Text = "Create an account",
        };
        registerBtn.Clicked += nav.NavigateTo<RegisterView>;

        container.Add(emailLabel, emailField, passwordLabel, passwordField, errorLabel, loginBtn, registerBtn);
        SetContent(container);
    }
}