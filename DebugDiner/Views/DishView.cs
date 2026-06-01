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

        var all = menu.GetItemsAsync().GetAwaiter().GetResult().ToList();
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

        tabView.AddTab(CreateTab("Search", all.ToList(), nav, isAdmin, searchable: true), false);

        SetContent(tabView);
    }

    private static TabView.Tab CreateTab(string title, List<DishEntity> dishes, INavigationService nav,
        bool isAdmin = false, bool searchable = false)
    {
        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

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
        }

        var sortColumn = 0;
        var ascending = true;

        // De rijen die op dit moment zichtbaar zijn (gesorteerd en/of gefilterd).
        var rows = searchable ? new List<DishEntity>(dishes) : SortDishes(dishes, sortColumn, ascending);

        void FillRows()
        {
            table.Rows.Clear();

            foreach (var dish in rows)
            {
                if (isAdmin)
                {
                    table.Rows.Add(dish.Id, dish.Name, dish.Price, dish.DishCategory.ToString(), dish.AllergenInfo,
                        dish.Description, "[Edit]", "[Delete]");
                }
                else
                {
                    table.Rows.Add(dish.Id, dish.Name, dish.Price, dish.DishCategory.ToString(), dish.AllergenInfo,
                        dish.Description);
                }
            }
        }

        FillRows();

        var tableView = new TableView(table)
        {
            X = 5,
            Y = searchable ? 4 : 3,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 6,
            ColorScheme = LayoutView.DefaultColorScheme
        };

        // Gebruik de "S" toets in een kolom waar de marker in staat om er op te sorteren.
        // Werkt zowel in de normale tabs als in de Search-tab, zolang de focus op de tabel
        // staat (en dus niet in de zoekbalk).
        tableView.KeyPress += (args) =>
        {
            if (args.KeyEvent.Key != Key.s && args.KeyEvent.Key != Key.S)
            {
                return;
            }

            var col = tableView.SelectedColumn;

            if (col >= 6)
            {
                return;
            }

            sortColumn = col;

            if (sortColumn == col)
            {
                ascending = !ascending;
            }
            else
            {
                ascending = true;
            }

            var selectedColumn = tableView.SelectedColumn;
            var selectedRow = tableView.SelectedRow;

            rows = SortDishes(rows, col, ascending);
            FillRows();
            tableView.Update();

            tableView.SelectedColumn = selectedColumn;
            tableView.SelectedRow = selectedRow;

            args.Handled = true;
        };

        if (searchable)
        {
            var searchLabel = new Label(5, 1, "Search: ");

            var searchField = new TextField
            {
                X = Pos.Right(searchLabel),
                Y = 1,
                Width = 50,
            };

            var resultLabel = new Label(5, 2, $"Found {rows.Count} dishes");

            searchField.TextChanged += _ =>
            {
                var query = searchField.Text?.ToString()?.Trim() ?? "";

                rows = string.IsNullOrEmpty(query)
                    ? new List<DishEntity>(dishes)
                    : dishes.Where(d => DishMatches(d, query)).ToList();

                FillRows();
                resultLabel.Text = $"Found {rows.Count} dishes";
                resultLabel.SetNeedsDisplay();
                tableView.Update();
            };

            container.Add(searchLabel, searchField, resultLabel, tableView);
        }
        else
        {
            var totalLabel = new Label(5, 1, $"Total {title} dishes: {dishes.Count}");

            container.Add(totalLabel, tableView);
        }

        tableView.CellActivated += (args) =>
        {
            if (args.Row >= rows.Count)
            {
                return;
            }
            var dish = rows[args.Row];

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

        return new TabView.Tab(title, container);
    }

    private static bool DishMatches(DishEntity dish, string query)
    {
        var haystack = string.Join(" ",
            dish.Id,
            dish.Name,
            dish.Price,
            dish.DishCategory,
            dish.AllergenInfo,
            dish.Description);

        return haystack.Contains(query, StringComparison.OrdinalIgnoreCase);
    }

    private static List<DishEntity> SortDishes(List<DishEntity> dishes, int column, bool ascending)
    {
        Func<DishEntity, object> keySelector = column switch
        {
            0 => d => d.Id,
            1 => d => d.Name,
            2 => d => d.Price,
            3 => d => d.DishCategory.ToString(),
            4 => d => d.AllergenInfo,
            5 => d => d.Description,
            _ => d => d.Id,
        };

        return ascending
            ? dishes.OrderBy(keySelector).ToList()
            : dishes.OrderByDescending(keySelector).ToList();
    }
}
