using System.Collections.ObjectModel;
using DebugDiner.Services;
using Terminal.Gui;

namespace DebugDiner;

public class BaseView : View
{
    protected FrameView HeaderFrame { get; }
    protected ListView NavigationMenu { get; }
    protected FrameView ContentFrame { get; }
    protected Label HeaderLabel { get; }

    public BaseView(INavigationService nav)
    {
        X = 0;
        Y = 0;
        Width = Dim.Fill();
        Height = Dim.Fill();
        ColorScheme = LayoutView.DefaultColorScheme;

        HeaderFrame = LayoutView.CreateHeaderFrame();
        HeaderLabel = LayoutView.CreateHeaderLabel("Debug Diner - Restaurant Reservation System");
        HeaderFrame.Add(HeaderLabel);
        Add(HeaderFrame);

        var navPanel = new View
        {
            X = 0,
            Y = Pos.Bottom(HeaderFrame),
            Width = 20,
            Height = Dim.Fill(),
            ColorScheme = LayoutView.DefaultColorScheme,
        };

        NavigationMenu = new ListView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill() - 2,
            ColorScheme = LayoutView.DefaultColorScheme,
        };

        var backBtn = new Button
        {
            X = 0,
            Y = Pos.Bottom(NavigationMenu),
            Text = "< Back",
        };
        backBtn.Clicked += nav.NavigateBack;

        var exitBtn = new Button
        {
            X = 0,
            Y = Pos.Bottom(backBtn),
            Text = "Exit",
        };
        exitBtn.Clicked += () => Terminal.Gui.Application.RequestStop();

        navPanel.Add(NavigationMenu, backBtn, exitBtn);
        Add(navPanel);

        ContentFrame = new FrameView
        {
            X = Pos.Right(navPanel),
            Y = Pos.Bottom(HeaderFrame),
            Width = Dim.Fill(),
            Height = Dim.Fill(),
            ColorScheme = LayoutView.DefaultColorScheme,
        };
        Add(ContentFrame);
    }

    protected void SetNavigationItems(params string[] items)
    {
        if (NavigationMenu.Source is null || NavigationMenu.Source.Count == 0)
        {
            NavigationMenu.SetSource(new ObservableCollection<string>(items));
            return;
        }
        var collection = NavigationMenu.Source.ToList();
        List<string> navItems = [];
        foreach (string item in collection)
        {
            navItems.Add(item);
        }
        navItems.AddRange(items);
        NavigationMenu.SetSource(new ObservableCollection<string>(navItems));
    }

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

    protected void SetHeaderTitle(string title)
    {
        HeaderLabel.Text = title;
    }

    protected void SetContentTitle(string title)
    {
        ContentFrame.Title = title;
    }
}
