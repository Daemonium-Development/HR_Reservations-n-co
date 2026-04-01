using System.Collections.ObjectModel;
using Terminal.Gui;

namespace DebugDiner;

public class BaseView : Window
{
    protected FrameView HeaderFrame { get; }
    protected ListView NavigationMenu { get; }
    protected FrameView ContentFrame { get; }
    protected Label HeaderLabel { get; }

    public BaseView()
    {
        Title = "Debug Diner - Restaurant Reservation System";
        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();
        ColorScheme = LayoutView.DefaultColorScheme;

        // Create layout components using LayoutView factory methods
        HeaderFrame = LayoutView.CreateHeaderFrame();
        HeaderLabel = LayoutView.CreateHeaderLabel("Debug Diner - Restaurant Reservation System");
        HeaderFrame.Add(HeaderLabel);
        Add(HeaderFrame);

        NavigationMenu = LayoutView.CreateNavigationMenu(HeaderFrame);
        Add(NavigationMenu);

        ContentFrame = LayoutView.CreateContentFrame(HeaderFrame, NavigationMenu);
        Add(ContentFrame);
    }

    // <summary>
    // Set the navigation menu items
    // </summary>
    protected void SetNavigationItems(params string[] items)
    {
        NavigationMenu.SetSource(new ObservableCollection<string>(items));
    }

    // <summary>
    // Clear and set new content in the main content area
    // </summary>
    protected void SetContent(View view)
    {
        ContentFrame.RemoveAll();
        if (view != null)
        {
            view.X = 0;
            view.Y = 0;
            view.Width = Dim.Fill();
            view.Height = Dim.Fill();
            ContentFrame.Add(view);
        }
    }

    // <summary>
    // Update the header title
    // </summary>
    protected void SetHeaderTitle(string title)
    {
        HeaderLabel.Text = title;
    }

    // <summary>
    // Update the content frame title
    // </summary>
    protected void SetContentTitle(string title)
    {
        ContentFrame.Title = title;
    }
}