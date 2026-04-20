using Terminal.Gui;

namespace DebugDiner;

public class CreateDishView : BaseView
{
    public CreateDishView(INavigationService nav) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Create Dish");

        SetNavigationItems(
            "Home",
            "Create Dish",
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

        var frame = new FrameView("Create Dish")
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

        var priceLabel = new Label
        {
            X = 5,
            Y = 5,
            Text = "Price:",
        };

        var priceField = new TextField
        {
            X = 20,
            Y = 5,
            Width = 20,
        };

        var descLabel = new Label
        {
            X = 5,
            Y = 7,
            Text = "Description:",
        };

        var descField = new TextField
        {
            X = 20,
            Y = 7,
            Width = 40,
        };

        var errorLabel = new Label
        {
            X = 5,
            Y = 10,
            Text = string.Empty,
        };

        var saveBtn = new Button
        {
            X = 5,
            Y = 12,
            Text = "Save",
        };

        saveBtn.Clicked += () =>
        {
            var name = nameField.Text.ToString() ?? string.Empty;
            var price = priceField.Text.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(price))
            {
                errorLabel.Text = "Name and price are required..";
                return;
            }

            MessageBox.Query("Complete", "Dish created !", "OK");
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
            priceLabel, priceField,
            descLabel, descField,
            errorLabel,
            saveBtn, cancelBtn
        );

        container.Add(frame);
        SetContent(container);
    }
}