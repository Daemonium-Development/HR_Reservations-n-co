using DebugDiner.Application;
using DebugDiner.Domain.Abstractions;
using DebugDiner.Infrastructure;
using DebugDiner.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Terminal.Gui;

namespace DebugDiner;

internal static class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var special = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        // var logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "debug-diner.log");
        // NOTE: If on mac, replace this with the line above
        var logFilePath = Path.Combine(special, "Debug Diner", "logs", "debug-diner.log");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel
            .Debug()
            .MinimumLevel
            .Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel
            .Override("Microsoft.Maui", LogEventLevel.Warning)
            .Enrich
            .FromLogContext()
            .WriteTo
            .File(path: logFilePath,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 30,
                outputTemplate:
                "[{Timestamp:dd-MM-yyyy HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, config) =>
            {
                var env = ctx.HostingEnvironment;
                env.EnvironmentName = "Development";
                config
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((_, services) =>
            {
                services.AddLogging(builder =>
                {
                    builder.AddSerilog(dispose: true);
                });
                services.AddSingleton(Log.Logger);
                services.AddSingleton<INavigationService, NavigationService>();
                services.AddInfrastructure();
                services.AddApplication();

                services.AddTransient<WelcomeView>();
                services.AddTransient<LoginView>();
                services.AddTransient<RegisterView>();
                services.AddTransient<HomeView>();
                services.AddTransient<InformationView>();
                services.AddTransient<MakeReservationsView>();
            });

        var app = builder.Build();

        var db = app.Services.GetRequiredService<IDataService>();
        await db.StartAsync();

        // Initialize and run Terminal.Gui
        Terminal.Gui.Application.Init();
        // NOTE: StopReportingMouseMoves prevents continuous mouse-position escape sequences
        // ([M#w etc.) from leaking into focused TextFields as raw text in xterm-compatible terminals.
        Terminal.Gui.Application.Driver.StopReportingMouseMoves();
        var nav = app.Services.GetRequiredService<INavigationService>();
        nav.SetContentArea(Terminal.Gui.Application.Top!);
        nav.NavigateTo<WelcomeView>();
        Terminal.Gui.Application.Run();

        return 0;
    }
}
