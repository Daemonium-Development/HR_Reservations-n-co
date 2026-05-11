using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;
using System.Collections.ObjectModel;
using Terminal.Gui;

namespace DebugDiner;

public class InformationView : BaseView
{
    private readonly UserEntity? _user;
    private readonly List<ReservationEntity> _reservations;

    public InformationView(INavigationService nav, IReservationRepository reservationRepository) : base(nav)
    {
        _user = AppState.CurrentUser;

        if (_user is not null)
        {
            var all = reservationRepository.GetItemsAsync().GetAwaiter().GetResult();
            _reservations = all.Where(r => r.UserId == _user.Id).ToList();
        }
        else
        {
            _reservations = [];
        }

        SetHeaderTitle("Debug Diner | User information");

        NavigationMenu.OpenSelectedItem += (ListViewItemEventArgs e) =>
        {
            switch (e.Item)
            {
                case 0: nav.NavigateTo<HomeView>(); break;
                case 1: nav.NavigateTo<MakeReservationsView>(); break;
                case 2: nav.NavigateTo<ReservationsView>(); break;
                case 4:
                    AppState.CurrentUser = null;
                    nav.NavigateTo<WelcomeView>();
                    break;
            }
        };

        SetContent(CreateInformationContent());
    }

    private static Label CreateLabelRow(string label, string value, int y)
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

        var currentY = 1;

        var name = _user is null ? "<not logged in>" : (string.IsNullOrWhiteSpace(_user.Name) ? "<empty>" : _user.Name);
        var nameLabel = CreateLabelRow("Name", name, currentY);
        currentY += 2;
        frame.Add(nameLabel);

        if (_reservations.Count > 0)
        {
            var reservationsLabel = new Label("Reservations:")
            {
                X = 2,
                Y = currentY
            };
            currentY += 1;

            var reservationStrings = new List<string>();
            foreach (var r in _reservations)
            {
                reservationStrings.Add(
                    $"#{r.Id} | Table {r.TableId} | {r.StartTime:dd-MM-yyyy HH:mm} - {r.EndTime:HH:mm} | {r.Guests} guests | {r.Status}"
                );
            }

            var reservationList = new ListView(reservationStrings)
            {
                X = 2,
                Y = currentY,
                Width = Dim.Fill() - 4,
                Height = 10
            };
            reservationList.SetSource(new ObservableCollection<string>(reservationStrings));

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
