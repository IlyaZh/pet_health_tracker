using ArchieHealthTracker.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Handlers;

public class UpdateHandler
{
    private readonly IUserService _userService;
    private readonly ILogger<UpdateHandler> _logger;

    public UpdateHandler(IUserService userService, ILogger<UpdateHandler> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    public async Task HandlerAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Message is { } message && message.Text is { } messageText)
        {
            await HandleMessageAsync(botClient, message, ct);
        }
    }
    
    private async Task HandleMessageAsync(ITelegramBotClient botClient, Message message, CancellationToken ct)
    {
        var from = message.From!;
        
        // Вызываем бизнес-логику
        var (user, isNew) = await _userService.RegisterUserAsync(from.Id, from.FirstName, from.Username);

        if (isNew)
        {
            await botClient.SendMessage(message.Chat.Id, 
                $"Привет, {user.FirstName}! Я новый трекер для Арчи. Добро пожаловать!", cancellationToken: ct);
        }
        else
        {
            await botClient.SendMessage(message.Chat.Id, 
                $"С возвращением, {user.FirstName}. Жду команды.", cancellationToken: ct);
        }
    }
}