using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using Terminal.Gui;

namespace DebugDiner;

public class UpdateDishView : BaseView
{
    public UpdateDishView(INavigationService nav, IMenuRepository menuRepository) : base(nav)
    {
        var selectedDish = AppState.SelectedDish;

        SetHeaderTitle("Debug Diner | Admin Panel");
        SetContentTitle("Update Dish");

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
            Text = selectedDish.Name,
        };
        nameInput.GetCurrentHeight(out var nameInputHeight);

        var priceLabel = new Label
        {
            X = 0,
            Y = nameInputHeight + nameInput.Y,
            Text = "Price:",
        };
        priceLabel.GetCurrentHeight(out var priceLabelHeight);

        var priceInput = new TextField
        {
            X = 0,
            Y = priceLabelHeight + priceLabel.Y,
            Width = Dim.Fill(),
            Text = selectedDish.Price.ToString(),
        };
        priceInput.GetCurrentHeight(out var priceInputHeight);

        var descriptionLabel = new Label
        {
            X = 0,
            Y = priceInputHeight + priceInput.Y,
            Text = "Description:",
        };
        descriptionLabel.GetCurrentHeight(out var descriptionLabelHeight);

        var descriptionInput = new TextField
        {
            X = 0,
            Y = descriptionLabelHeight + descriptionLabel.Y,
            Width = Dim.Fill(),
            Text = selectedDish.Description,
        };
        descriptionInput.GetCurrentHeight(out var descriptionInputHeight);

        var categoryTable = new DataTable();

        var categoryList = Enum.GetNames<DishCategory>().ToList();

        foreach (var category in categoryList)
        {
            categoryTable.Columns.Add(category);
        }

        var combinedCategories = string.Join(", ", Enum.GetNames<DishCategory>().ToList());

        var categoryLabel = new Label
        {
            X = 0,
            Y = descriptionInputHeight + descriptionInput.Y,
            Text = $"Category ({combinedCategories}):",
        };
        categoryLabel.GetCurrentHeight(out var categoryLabelHeight);

        var categoryInput = new TextField
        {
            X = 0,
            Y = categoryLabelHeight + categoryLabel.Y,
            Width = Dim.Fill(),
            Text = selectedDish.DishCategory.ToString(),
        };
        categoryInput.GetCurrentHeight(out var categoryInputHeight);

        var allergenLabel = new Label
        {
            X = 0,
            Y = categoryInputHeight + categoryInput.Y,
            Text = "Allergen Info:",
            Width = Dim.Fill()
        };
        allergenLabel.GetCurrentHeight(out var allergenLabelHeight);

        var allergenInput = new TextField
        {
            X = 0,
            Y = allergenLabelHeight + allergenLabel.Y,
            Width = Dim.Fill(),
            Text = selectedDish.AllergenInfo,
        };
        allergenInput.GetCurrentHeight(out var allergenInputHeight);

        var submitBtn = new Button
        {
            X = 0,
            Y = (allergenInputHeight + allergenInput.Y) + 1,
            AutoSize = true,
            Text = $"Update {selectedDish.Name}",
        };

        submitBtn.Clicked += () =>
        {
            selectedDish.Name = nameInput.Text.ToString() ?? selectedDish.Name;

            if (Decimal.TryParse(priceInput.Text.ToString(), out _))
            {
                selectedDish.Price = Decimal.Parse(priceInput.Text.ToString() ?? selectedDish.Price.ToString());
            }

            selectedDish.Description = descriptionInput.Text.ToString() ?? selectedDish.Description;

            if (Enum.TryParse<DishCategory>(categoryInput.Text.ToString(), out _))
            {
                selectedDish.DishCategory = Enum.Parse<DishCategory>(categoryInput.Text.ToString()
                                                                     ?? selectedDish.DishCategory.ToString());
            }

            selectedDish.AllergenInfo = allergenInput.Text.ToString() ?? selectedDish.AllergenInfo;

            try
            {
                menuRepository.Update([selectedDish]).GetAwaiter().GetResult();
                nav.NavigateBack();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to update dish: {ex.Message}", "OK");
            }
        };

        container.Add(nameLabel, nameInput, priceLabel, priceInput, descriptionLabel, descriptionInput,
            categoryLabel, categoryInput, allergenLabel, allergenInput, submitBtn);

        SetContent(container);
    }

}
