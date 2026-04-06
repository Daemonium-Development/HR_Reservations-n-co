using Terminal.Gui;

namespace DebugDiner;

// <summary>
// Welcome view - First screen shown to the user.
// Displays welcome message and prompts user to press Enter to continue.
// </summary>
public class WelcomeView : BaseView
{
    public WelcomeView() : base()
    {
        SetHeaderTitle("Welcome to Debug Diner");
        SetContentTitle("Welcome");

        // Create a container for the welcome content
        var welcomeContainer = new View
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        // Welcome message
        var welcomeLabel = new Label
        {
            X = 10,
            Y = 3,
            Text = "Welcome to Debug Diner",
        };

        var subtitleLabel = new Label
        {
            X = 8,
            Y = 5,
            Text = "Restaurant Reservation System",
        };

        var instructionLabel = new Label
        {
            X = 11,
            Y = 8,
            Text = "Press Enter to continue",
        };

        // Continue button
        var continueBtn = new Button
        {
            X = 16,
            Y = 10,
            Text = "Continue",
            IsDefault = true,
        };

        welcomeContainer.Add(welcomeLabel, subtitleLabel, instructionLabel, continueBtn);
        SetContent(welcomeContainer);

        // Hide navigation menu for welcome screen
        // NavigationMenu.Visible = false;
    }
}
