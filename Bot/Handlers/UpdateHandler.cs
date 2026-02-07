using ArchieHealthTracker.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Handlers;

public class UpdateHandler
{
    private readonly IUserService _userService;
    private readonly CommandExecutor _commandExecutor; 
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(
        IUserService userService, 
        CommandExecutor commandExecutor, 
        ILogger<UpdateHandler> logger)
    {
        _userService = userService;
        _commandExecutor = commandExecutor;
        _logger = logger;
    }

    public async Task HandlerAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Message is { Text: { } messageText } message)
        {
            await HandleUpdateAsync(botClient, message, messageText, ct);
        }
        else if (update.CallbackQuery is { Data: { } callbackData } callback)
        {
            await HandleUpdateAsync(botClient, callback.Message!, callbackData, ct);
        }
    }
    
    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Message message, string text, CancellationToken ct)
    {
        var from = message.From;
        if (from == null)
        {
            _logger.LogWarning("Получено сообщение без отправителя (From is null). MessageId: {Id}", message.Id);
            return;
        }
        
        var (user, isNew) = await _userService.RegisterUserAsync(from.Id, from.FirstName, from.Username);
        
        _logger.LogInformation("Обработка сообщения от {UserId}: {Text}", user.TelegramId, text);
        
        await _commandExecutor.ExecuteCommand(text, botClient, message, user, ct);
    }
}