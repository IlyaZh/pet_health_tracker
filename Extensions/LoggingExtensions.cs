using Serilog;
using Serilog.Formatting.Compact;

namespace ArchieHealthTracker.Extensions;

public static class LoggingExtensions
{
    public static void AddSerilogLogging(this IHostBuilder host)
    {
        host.UseSerilog((context, services, config) =>
        {
            var path = context.Configuration["Logging:File:Path"] ?? "Logs/health-tracker-.log";
            var intervalDays = context.Configuration.GetValue("Logging:File:RetainedDays", 7);

            config
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.FromLogContext()
                // .Enrich.With()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    formatter: new CompactJsonFormatter(),
                    path: path,
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: intervalDays,
                    fileSizeLimitBytes: 10 * 1024 * 1024,
                    rollOnFileSizeLimit: true
                );
        });
    }
}