using ArchieHealthTracker.Bot.Configuration;
using ArchieHealthTracker.Bot.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ArchieHealthTracker.Bot.Services;

public class BotService(
    ITelegramBotClient botClient,
    IOptions<BotConfiguration> config,
    ILogger<BotService> logger,
    IServiceScopeFactory scopeFactory,
    IHostEnvironment env)
    : BackgroundService
{
    private readonly BotConfiguration _config = config.Value;
    private readonly IHostEnvironment _env = env;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var mode = _config.UpdateMode;
        var webhookUrl = _config.WebhookUrl;

        logger.LogInformation("Starting bot in {Mode} mode", mode);


        if (UpdateMode.Webhook == mode && !string.IsNullOrEmpty(webhookUrl))
        {
            logger.LogInformation("Setting webhook to {Url}", _config.WebhookUrl);
            await botClient.SetWebhook(
                _config.WebhookUrl,
                allowedUpdates: Array.Empty<UpdateType>(),
                cancellationToken: stoppingToken,
                secretToken: _config.SecretToken
            );
        }
        else
        {
            await botClient.DeleteWebhook(cancellationToken: stoppingToken);

            botClient.StartReceiving(
                HandleUpdateAsync,
                HandlePollingErrorAsync,
                new ReceiverOptions
                {
                    AllowedUpdates = []
                },
                stoppingToken
            );
        }


        var me = await botClient.GetMe(stoppingToken);
        logger.LogInformation($"The bot {me.Username} has been started");

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();

        var allowedUsers = _config.AllowedUsers;

        var handler = scope.ServiceProvider.GetRequiredService<UpdateHandler>();

        try
        {
            await handler.HandlerAsync(botClient, update, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling update");
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken ct)
    {
        logger.LogError(exception, "Ошибка Telegram API");
        return Task.CompletedTask;
    }
}