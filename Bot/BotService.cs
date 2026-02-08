using ArchieHealthTracker.Bot.Handlers;
using ArchieHealthTracker.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot;

public class BotService : BackgroundService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<BotService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting bot");
        
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = []
        };
        _botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: stoppingToken
        );
        
        var me = await _botClient.GetMe(stoppingToken);
        _logger.LogInformation($"Бот {me.Username} успешно запущен и слушает сообщения.");
        
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public BotService(IConfiguration config, ILogger<BotService> logger, IServiceScopeFactory scopeFactory, IOptions<BotConfiguration> options)
    {
        _botClient = new TelegramBotClient(options.Value.Token);
        _logger = logger;
        _configuration = config;
        _scopeFactory = scopeFactory;
    }
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        
        var allowedUsers = _configuration.GetSection("BotConfiguration:AllowedUsers").Get<HashSet<long>>();
        
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
    
    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Ошибка Telegram API");
        return Task.CompletedTask;
    }
}