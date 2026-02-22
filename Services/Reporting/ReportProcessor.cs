using ArchieHealthTracker.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace ArchieHealthTracker.Services.Reporting;

public class ReportProcessor : BackgroundService
{
    private readonly ILogger<ReportProcessor> _logger;
    private readonly IReportQueue _queue;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITelegramBotClient _botClient; 

    public ReportProcessor(
        ILogger<ReportProcessor> logger,
        IReportQueue queue,
        IServiceProvider serviceProvider,
        ITelegramBotClient botClient
        )
    {
        _logger = logger;
        _queue = queue;
        _serviceProvider = serviceProvider;
        _botClient = botClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[ReportProcessor] Report Processor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var item = await _queue.DequeueReportAsync(stoppingToken);

                using var scope = _serviceProvider.CreateScope();
                var healthService = scope.ServiceProvider.GetRequiredService<IHealthService>();
                var reportGenerator = scope.ServiceProvider.GetRequiredService<IReportGenerator<string>>();
                
                _logger.LogInformation($"[ReportProcessor] Report Generator started.");
                
                var context = await healthService.PrepareReportContextAsync(item.Request, stoppingToken);
                var report = await reportGenerator.GenerateAsync(context, stoppingToken);

                await _botClient.SendMessage(
                    item.ChatId,
                    report,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: stoppingToken
                    );
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[ReportProcessor] Error processing report task");
            }
        }
    }
}