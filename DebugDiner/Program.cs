using DebugDiner.Domain.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace DebugDiner;

internal static class Program
{
    internal static async Task<int> Main(string[] args)
    {
        var builder = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, config) =>
            {
                var env = ctx.HostingEnvironment;
                env.EnvironmentName = "Development";
                config
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureLogging((ctx, logging) =>
            {
                var logFilePath = Path.Combine(ctx.HostingEnvironment.ContentRootPath, "logs", "debug-diner.log");
                var logger = new LoggerConfiguration()
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
                        "[{Timestamp:dd-MM-yyyy HH:mm:ss} {Level:u3}] {SourceContext}: {Message:lj}{NewLine}{Exception}");
                logging.AddSerilog(logger.CreateLogger(), dispose: true);
            })
            .ConfigureServices((_, services) =>
            {
                // services.AddDomain();
                // services.AddInfrastructure();
            });
        
        var app = builder.Build();
        
        return 0;
    }
}