using ArchieHealthTracker.Bot.Handlers;
using ArchieHealthTracker.Configuration;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ArchieHealthTracker.Bot;

public class BotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly BotConfiguration _configuration;
    private readonly IHostEnvironment _env;
    private readonly ILogger<BotService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public BotService(
        ITelegramBotClient botClient,
        IOptions<BotConfiguration> config,
        ILogger<BotService> logger,
        IServiceScopeFactory scopeFactory,
        IHostEnvironment env
    )
    {
        _botClient = botClient;
        _logger = logger;
        _configuration = config.Value;
        _scopeFactory = scopeFactory;
        _env = env;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mode = _configuration.UpdateMode;
        var webhookUrl = _configuration.WebhookUrl;

        _logger.LogInformation("Starting bot in {Mode} mode", mode);

        if (UpdateMode.Webhook == mode && !string.IsNullOrEmpty(webhookUrl))
        {
            _logger.LogInformation("Setting webhook to {Url}", _configuration.WebhookUrl);
            await _botClient.SetWebhook(
                url: _configuration.WebhookUrl,
                allowedUpdates: Array.Empty<UpdateType>(),
                cancellationToken: stoppingToken
            );
        }
        else
        {
            await _botClient.DeleteWebhook(cancellationToken: stoppingToken);

            _botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                errorHandler: HandlePollingErrorAsync,
                receiverOptions: new ReceiverOptions
                {
                    AllowedUpdates = []
                },
                cancellationToken: stoppingToken
            );
        }


        var me = await _botClient.GetMe(stoppingToken);
        _logger.LogInformation($"The bot {me.Username} has been started");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();

        var allowedUsers = _configuration.AllowedUsers;

        var handler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();

        try
        {
            await handler.HandlerAsync(botClient, update, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling update");
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken ct)
    {
        _logger.LogError(exception, "Ошибка Telegram API");
        return Task.CompletedTask;
    }
}