using Terminal.Gui;
using DebugDiner.Domain.Abstractions;
using DebugDiner.Services;

namespace DebugDiner;

public class DeleteReservationView : BaseView
{
    public DeleteReservationView(INavigationService nav, IReservationRepository repo) : base(nav)
    {
        SetHeaderTitle("Debug Diner | Delete Reservation");
        SetContentTitle("Delete Reservation");

        var reservation = AppState.SelectedReservation;

        if (reservation is null)
        {
            nav.NavigateBack();
            return;
        }

        var container = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        var questionLabel = new Label
        {
            X = Pos.Center(),
            Y = 5,
            Text = "Are you sure you want to delete this reservation?"
        };

        var detailLabel = new Label
        {
            X = Pos.Center(),
            Y = questionLabel.Y + 1,
            Text = $"Table {reservation.TableId} — {reservation.StartTime.ToString("dd-MM-yyyy HH:mm")}"
        };

        var yesBtn = new Button
        {
            X = 5,
            Y = detailLabel.Y + 3,
            AutoSize = true,
            Text = "Yes, delete reservation"
        };

        yesBtn.Clicked += () =>
        {
            try
            {
                repo.Delete([reservation]).GetAwaiter().GetResult();
                AppState.SelectedReservation = null;
                nav.NavigateBack();
            }
            catch (Exception ex)
            {
                MessageBox.ErrorQuery("Error", $"Failed to delete reservation: {ex.Message}", "OK");
            }
        };

        var noBtn = new Button
        {
            X = Pos.Right(yesBtn) + 5,
            Y = yesBtn.Y,
            AutoSize = true,
            Text = "No, go back"
        };

        noBtn.Clicked += nav.NavigateBack;

        container.Add(questionLabel, detailLabel, yesBtn, noBtn);
        SetContent(container);
    }
}
