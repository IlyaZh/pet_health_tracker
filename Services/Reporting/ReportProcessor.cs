using System.Text;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ArchieHealthTracker.Services.Reporting;

public class ReportProcessor : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<ReportProcessor> _logger;
    private readonly IReportQueue _queue;
    private readonly IServiceProvider _serviceProvider;

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

                _logger.LogInformation("[PdfGenerator] Received data: Weights: {W}, Medical: {M}, Symptoms: {S}",
                    context.WeightEntries?.Count() ?? 0,
                    context.MedicalEventsEntries?.Count() ?? 0,
                    context.SymptomEntries?.Count() ?? 0);


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