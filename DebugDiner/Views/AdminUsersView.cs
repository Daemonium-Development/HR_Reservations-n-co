using System.Data;
using DebugDiner.Services;
using DebugDiner.Domain.Abstractions;
using Terminal.Gui;

namespace DebugDiner;

public class AdminUsersView : BaseView
{
    public AdminUsersView(INavigationService nav, IUserRepository userRepository) : base(nav)
    {
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
            ColorScheme = LayoutView.DefaultColorScheme
        };

        List<UserEntity> users = [];

        void LoadUsers()
        {
            try
            {
                users = userRepository.GetItemsAsync().GetAwaiter().GetResult().ToList();
                totalLabel.Text = $"Total: {users.Count} users";
                table.Rows.Clear();
                foreach (var u in users)
                {
                    table.Rows.Add(u.Name, u.Email, u.Role.ToString(), "[Edit]", "[Delete]");
                }
                tableView.Table = table;
                tableView.SetNeedsDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to load users: {ex.Message}", "OK");
            }
        }

        tableView.CellActivated += (args) =>
        {
            if (args.Row >= users.Count)
            {
                return;
            }
            var user = users[args.Row];

            if (args.Col == 3) // Edit column
            {
                AppState.SelectedUser = user;
                nav.NavigateTo<UpdateUserView>();
            }
            else if (args.Col == 4) // Delete column
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

        LoadUsers();
    }
}
