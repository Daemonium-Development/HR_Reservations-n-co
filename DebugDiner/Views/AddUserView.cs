using Terminal.Gui;

namespace DebugDiner;

public class AddUserView : BaseView
{
    public AddUserView(INavigationService nav) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Add User");

        SetNavigationItems(
            "Home",
            "Users",
            "Add User",
            "Reservations",
            "Logout"
        );

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

        var errorLabel = new Label
        {
            X = 5,
            Y = 10,
            Text = string.Empty,
        };

        var createBtn = new Button
        {
            X = 5,
            Y = 12,
            Text = "Create",
        };

        createBtn.Clicked += () =>
        {
            var name = nameField.Text.ToString() ?? string.Empty;
            var email = emailField.Text.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                errorLabel.Text = "Name and email are mandatory";
                return;
            }

            MessageBox.Query("Success", "User created !", "OK");
            nav.NavigateTo<HomeView>();
        };

        var cancelBtn = new Button
        {
            X = 15,
            Y = 12,
            Text = "Cancel",
        };

        cancelBtn.Clicked += () => nav.NavigateTo<HomeView>();

        frame.Add(
            nameLabel, nameField,
            emailLabel, emailField,
            errorLabel,
            createBtn, cancelBtn
        );

        container.Add(frame);
        SetContent(container);
    }
}