using System.Collections.ObjectModel;
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
        var headerLabel = new Label(5, 3, "Name                     Email                              Role");

        var usersList = new ListView
        {
            X = 5,
            Y = 4,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 6,
            ColorScheme = LayoutView.DefaultColorScheme
        };

        var container = new View { 
            X = 0, 
            Y = 0, 
            Width = Dim.Fill(), 
            Height = Dim.Fill() 
        };
        
        container.Add(totalLabel, headerLabel, usersList);
        SetContent(container);

        try
        {
            var users = userRepository.GetItemsAsync().GetAwaiter().GetResult().ToList();
            totalLabel.Text = $"Total: {users.Count} users";

            var items = users.Select(u => $"{u.Name,-24} {u.Email,-32} {u.Role}").ToList();
            usersList.SetSource(new ObservableCollection<string>(items));
        }
        catch (Exception ex)
        {
            MessageBox.ErrorQuery("Error", $"Failed to load users: {ex.Message}", "OK");
        }
    }
}