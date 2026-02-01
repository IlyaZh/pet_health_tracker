using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ArchieHealthTracker.Bot;

public class BotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<BotService> _logger;

    public BotService(IConfiguration config, ILogger<BotService> logger)
    {
        _logger = logger;
        var token = config["BotConfiguration:Token"] ??
                    throw new ArgumentNullException("Token for Bot not found in config");
        _botClient = new TelegramBotClient(token);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Bot started");
        
        // TODO: support polling for debug mode and webhook for production
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };
        
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
            );

        var me = await _botClient.GetMe(stoppingToken);
        _logger.LogInformation($"Bot started: @{me.Username}");
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message) return;
        if (message.Text is not {} messageText) return;
        
        var chatId  = message.Chat.Id;
        _logger.LogDebug($"Message received '{messageText}' in chat {chatId}");
        
        // Echo response just for testing
        await botClient.SendMessage(
            chatId: chatId,
            text: $"Handled:  {messageText}",
            cancellationToken: cancellationToken);
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n [{apiRequestException.ErrorCode}\n{apiRequestException.Message}]",
            _ =>  exception.ToString()
        };
        
        _logger.LogError(errorMessage);
        return Task.CompletedTask;
    }
}