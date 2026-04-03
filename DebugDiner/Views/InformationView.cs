using Terminal.Gui;
using System.Collections.Generic;

namespace DebugDiner;

public class InformationView : BaseView
{
    private readonly User _user;
    private readonly List<Reservation> _reservations;

    public InformationView(User user, List<Reservation> reservations)
    {
        _user = user;
        _reservations = reservations;

        SetHeaderTitle("Debug Diner | User information");
        SetContentTitle("User cetails");

        SetNavigationItems(
            "Home",
            "Make a reservation",
            "View my reservation(s)",
            "View user information",
            "Logout"
        );

        SetContent(CreateInformationContent());
    }

    private Label CreateLabelRow(string label, string value, int y)
    {
        return new Label($"{label}: {value}")
        {
            X = 2,
            Y = y
        };
    }

    private View CreateInformationContent()
    {
        var container = new View
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var frame = new FrameView("User details")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        int currentY = 1;

        var nameLabel = CreateLabelRow("Name", string.IsNullOrWhiteSpace(_user.Name) ? "<empty>" : _user.Name, currentY);
        currentY = Pos.Bottom(nameLabel) + 1;
        frame.Add(nameLabel);

        if (_reservations != null && _reservations.Count > 0)
        {
            var reservationsLabel = new Label(",Reservations:")
            {
                X = 2,
                Y = currentY
            };
            currentY = Pos.Bottom(reservationsLabel) + 1;

            var reservationStrings = new List<string>();
            foreach (var r in _reservations)
            {
                reservationStrings.Add(r.ToString()); 
            }

            var reservationList = new ListView(reservationStrings)
            {
                X = 2,
                Y = currentY,
                Width = Dim.Fill() - 4,
                Height = 10
            };

            frame.Add(reservationsLabel, reservationList);
        }
        else
        {
            var noResLabel = new Label("Could not find reservations..")
            {
                X = 2,
                Y = currentY
            };
            frame.Add(noResLabel);
        }

        container.Add(frame);
        return container;
    }

}