using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class CreateDishView : BaseView
{
    public CreateDishView(INavigationService nav, IMenuRepository menu) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Create Dish");

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

        var categoryLabel = new Label
        {
            X = 5,
            Y = 9,
            Text = "Category (Meat/Fish/Vegetarian/Vegan):",
        };

        var categoryField = new TextField
        {
            X = 5,
            Y = 10,
            Width = 30,
        };

        var errorLabel = new Label
        {
            X = 5,
            Y = 12,
            Text = string.Empty,
        };

        var saveBtn = new Button
        {
            X = 5,
            Y = 14,
            Text = "Save",
        };

        saveBtn.Clicked += () =>
        {
            var name = nameField.Text.ToString() ?? string.Empty;
            var priceText = priceField.Text.ToString() ?? string.Empty;
            var description = descField.Text.ToString() ?? string.Empty;
            var categoryText = categoryField.Text.ToString() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(priceText))
            {
                errorLabel.Text = "Name and price are required.";
                return;
            }

            if (!decimal.TryParse(priceText, out var price) || price < 0)
            {
                errorLabel.Text = "Price must be a valid positive number.";
                return;
            }

            if (!Enum.TryParse<DishCategory>(categoryText, ignoreCase: true, out var category))
            {
                errorLabel.Text = "Category must be: Meat, Fish, Vegetarian, or Vegan.";
                return;
            }

            var entity = new DishEntity
            {
                Id = 0,
                CreatedAt = DateTime.UtcNow,
                Name = name,
                Description = description,
                Price = price,
                DishCategory = category,
                AllergenInfo = string.Empty,
            };

            var result = menu.Create([entity]).GetAwaiter().GetResult();
            if (!result.Any())
            {
                errorLabel.Text = "Failed to save dish.";
                return;
            }

            MessageBox.Query("Complete", $"Dish '{name}' created.", "OK");
            nav.NavigateBack();
        };

        var cancelBtn = new Button
        {
            X = 15,
            Y = 14,
            Text = "Cancel",
        };

        cancelBtn.Clicked += nav.NavigateBack;

        frame.Add(
            nameLabel, nameField,
            priceLabel, priceField,
            descLabel, descField,
            categoryLabel, categoryField,
            errorLabel,
            saveBtn, cancelBtn
        );

        container.Add(frame);
        SetContent(container);
    }
}
