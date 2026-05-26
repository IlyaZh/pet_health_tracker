using ArchieHealthTracker.Application.Interfaces;
using ArchieHealthTracker.Domain.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArchieHealthTracker.Application.Services.Reporting;

public class ReportProcessor(
    ILogger<ReportProcessor> logger,
    IReportQueue queue,
    IServiceProvider serviceProvider,
    INotificationService client)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        logger.LogInformation("[ReportProcessor] Report Processor started.");

        while (!ct.IsCancellationRequested)
            try
            {
                var item = await queue.DequeueReportAsync(ct);

                using var scope = serviceProvider.CreateScope();
                var healthService = scope.ServiceProvider.GetRequiredService<IHealthService>();
                var generator = scope.ServiceProvider.GetRequiredKeyedService<IReportGenerator>(item.Format);

                logger.LogInformation("[ReportProcessor] Report Generator started.");

                var context = await healthService.PrepareReportContextAsync(item.Request, ct);

                var result = await Task.Run(() => generator.Generate(context, ct), ct);

                await client.SendAsync(
                    item.ChatId,
                    result,
                    ct
                );
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[ReportProcessor] Error processing report task");
            }
    }
}
