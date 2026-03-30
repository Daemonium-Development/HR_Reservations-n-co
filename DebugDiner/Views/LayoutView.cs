using Spectre.Console;

namespace DebugDiner;

class LayoutView 
{
    public LayoutView() : base()
    {
        AnsiConsole.MarkupLine("[bold yellow]Welcome to:[/]");
        var figlet = new FigletText("Debug Diner!"){
            Color =Color.Green,
            Justification = Justify.Center
        };
        AnsiConsole.Write(figlet);
        AnsiConsole.WriteLine();

        // Show order summary
        var panel = new Panel(
                new Rows(
                    new Markup($"[bold]Customer:[/] test")))
            .Header("[yellow]Login[/]")
            .Border(BoxBorder.Rounded);
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }
}