using ArchieHealthTracker.Application.Services;
using ArchieHealthTracker.Application.Services.Reporting;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ArchieHealthTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IHealthService, HealthService>();

        services.AddSingleton<IUserSessionService, UserSessionService>();
        services.AddSingleton<IReportQueue, ReportQueue>();
        services.AddHostedService<ReportProcessor>();

        services.AddKeyedScoped<IReportGenerator, TelegramReportGenerator>(ReportFormat.Telegram);
        services.AddKeyedScoped<IReportGenerator, PdfReportGenerator>(ReportFormat.Pdf);

        return services;
    }
}