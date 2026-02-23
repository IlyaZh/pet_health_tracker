using System.Text;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

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
                var generator = scope.ServiceProvider.GetRequiredKeyedService<IReportGenerator>(item.Format);

                _logger.LogInformation($"[ReportProcessor] Report Generator started.");

                var context = await healthService.PrepareReportContextAsync(item.Request, stoppingToken);

                var result = await generator.GenerateAsync(context, stoppingToken);

                if (item.Format == ReportFormat.Telegram)
                {
                    await _botClient.SendMessage(
                        item.ChatId,
                        Encoding.UTF8.GetString(result.Content),
                        parseMode: ParseMode.Markdown,
                        cancellationToken: stoppingToken
                    );
                }
                else
                {
                    using var ms = new MemoryStream(result.Content);
                    await _botClient.SendDocument(
                        item.ChatId,
                        InputFile.FromStream(ms, result.FileName),
                        caption: "Твой отчет готов! 🐾",
                        cancellationToken: stoppingToken
                    );
                }
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