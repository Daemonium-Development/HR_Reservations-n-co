using DebugDiner.Domain.Abstractions;
using System.Collections.ObjectModel;
using DebugDiner.Services;
using System.Data;
using Terminal.Gui;

namespace DebugDiner;

public class DishView : BaseView
{
    public DishView(INavigationService nav, IMenuRepository menu) : base(nav)
    {
        SetHeaderTitle("Menu");
        SetContentTitle("All Dishes");

        var isAdmin = AppState.CurrentUser?.Role == Role.Admin;

        var all = menu.GetItemsAsync().GetAwaiter().GetResult();
        Dictionary<object, List<DishEntity>> grouped = [];

        var rawGrouped = all.GroupBy(d => d.DishCategory)
            .ToDictionary(g => g.Key, g => g.ToList());

        grouped.Add("All", all.ToList());

        foreach (var (category, dishes) in rawGrouped)
        {
            grouped.Add(category, dishes);
        }

        var tabView = new TabView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        foreach (var (status, filtered) in grouped)
        {
            tabView.AddTab(CreateTab(status.ToString(), filtered, nav, isAdmin), false);
        }

        SetContent(tabView);
    }

    private static TabView.Tab CreateTab(string title, List<DishEntity> dishes, INavigationService nav,
        bool isAdmin = false)
    {
        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var totalLabel = new Label(5, 1, $"Total {title} dishes: {dishes.Count}");

        var table = new DataTable();

        table.Columns.Add("ID", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Columns.Add("Price", typeof(decimal));
        table.Columns.Add("Category", typeof(string));
        table.Columns.Add("Allergenes", typeof(string));
        table.Columns.Add("Description", typeof(string));

        if (isAdmin)
        {
            table.Columns.Add("Edit", typeof(string));
            table.Columns.Add("Delete", typeof(string));

            foreach (var dish in dishes)
            {
                table.Rows.Add(dish.Id, dish.Name, dish.Price, dish.DishCategory.ToString(), dish.AllergenInfo,
                    dish.Description, "[Edit]", "[Delete]");
            }
        }
        else
        {
            foreach (var dish in dishes)
            {
                table.Rows.Add(dish.Id, dish.Name, dish.Price, dish.DishCategory.ToString(), dish.AllergenInfo,
                    dish.Description);
            }
        }

        var tableView = new TableView(table)
        {
            X = 5,
            Y = 3,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 6,
            ColorScheme = LayoutView.DefaultColorScheme
        };

        tableView.CellActivated += (args) =>
        {
            if (args.Row >= dishes.Count)
            {
                return;
            }
            var dish = dishes[args.Row];

            if (args.Col == 6) // Edit column
            {
                AppState.SelectedDish = dish;
                nav.NavigateTo<UpdateDishView>();
            }
            else if (args.Col == 7) // Delete column
            {
                AppState.SelectedDish = dish;
                nav.NavigateTo<DeleteDishView>();
            }
        };

        container.Add(totalLabel, tableView);

        return new TabView.Tab(title, container);
    }
}
