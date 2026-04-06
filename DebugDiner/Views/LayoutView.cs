using System.Collections.ObjectModel;
using Terminal.Gui;

namespace DebugDiner;

// <summary>
// Layout factory for all views.
// Creates and configures all layout components: header, sidebar, and content areas.
// Purely responsible for layout structure and styling.
// </summary>
public static class LayoutView
{
    // <summary>
    // Creates the header frame with styling
    // </summary>
    public static ColorScheme DefaultColorScheme => new ColorScheme()
    {
        Normal = new Terminal.Gui.Attribute(Color.White, Color.Black),
        Focus = new Terminal.Gui.Attribute(Color.Black, Color.Cyan),
        HotNormal = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Black),
        HotFocus = new Terminal.Gui.Attribute(Color.BrightYellow, Color.Cyan),
    }!;

    // <summary>
    // Creates the header frame with styling
    // </summary>
    public static FrameView CreateHeaderFrame()
    {
        return new FrameView
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = 3,
            ColorScheme = DefaultColorScheme,
        };
    }

    // <summary>
    // Creates the header label with default positioning
    // </summary>
    public static Label CreateHeaderLabel(string text)
    {
        return new Label
        {
            X = 1,
            Y = 0,
            Text = text,
        };
    }


}
