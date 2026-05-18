using System.Data;
using Terminal.Gui;
using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;

namespace DebugDiner;

public class AdminUsersView : BaseView
{
    private List<UserEntity> _users = [];

    public AdminUsersView(INavigationService nav, IUserRepository userRepository) : base(nav)
    {
        const int EditCol   = 3;
        const int DeleteCol = 4;

        SetHeaderTitle("Debug Diner | Admin Panel");
        SetContentTitle("All Registered Users");

        var totalLabel = new Label(5, 1, "Total: 0 users");

        var table = new DataTable();
        table.Columns.Add("Name");
        table.Columns.Add("Email");
        table.Columns.Add("Role");
        table.Columns.Add("Edit");
        table.Columns.Add("Delete");

        var tableView = new TableView
        {
            X = 5,
            Y = 3,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 6,
            ColorScheme = LayoutView.DefaultColorScheme,
            Table = table
        };

        tableView.CellActivated += (args) =>
        {
            if (args.Row >= _users.Count)
            {
                return;
            }

            var user = _users[args.Row];

            if (args.Col == EditCol)
            {
                AppState.SelectedUser = user;
                nav.NavigateTo<UpdateUserView>();
            }
            else if (args.Col == DeleteCol)
            {
                AppState.SelectedUser = user;
                nav.NavigateTo<DeleteUserView>();
            }
        };

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };
        container.Add(totalLabel, tableView);
        SetContent(container);

        LoadUsers(userRepository, table, totalLabel, tableView);
    }

    private void LoadUsers(IUserRepository userRepository, DataTable table, Label totalLabel, TableView tableView)
    {
        try
        {
            _users = [.. userRepository.GetItemsAsync().GetAwaiter().GetResult()];
            totalLabel.Text = $"Total: {_users.Count} users";
            table.Rows.Clear();
            foreach (var u in _users)
            {
                table.Rows.Add(u.Name, u.Email, u.Role.ToString(), "[Edit]", "[Delete]");
            }
            tableView.SetNeedsDisplay();
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to load users: {ex.Message}", "OK");
        }
    }
}
