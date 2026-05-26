using Serilog;
using Serilog.Formatting.Compact;

namespace ArchieHealthTracker.WebService.Extensions;

public static class LoggingExtensions
{
    public static void AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                new CompactJsonFormatter(),
                configuration["Logging:File:Path"] ?? "Logs/health-tracker-.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: configuration.GetValue("Logging:File:RetainedDays", 7),
                fileSizeLimitBytes: 10 * 1024 * 1024,
                rollOnFileSizeLimit: true
            )
            .CreateLogger();

        services.AddSerilog();
    }
}
