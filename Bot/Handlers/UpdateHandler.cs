using ArchieHealthTracker.Configuration;
using ArchieHealthTracker.Repositories;
using ArchieHealthTracker.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Handlers;

public class UpdateHandler
{
    private readonly IUserService _userService;
    private readonly CommandExecutor _commandExecutor;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly HashSet<long> _allowedUsers;

    public UpdateHandler(
        IUserService userService,
        CommandExecutor commandExecutor,
        ILogger<UpdateHandler> logger,
        IOptions<BotConfiguration> botConfiguration)
    {
        _userService = userService;
        _commandExecutor = commandExecutor;
        _logger = logger;
        _allowedUsers = botConfiguration.Value.AllowedUsers;

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

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Message message, string text,
        CancellationToken ct)
    {
        var from = message.From;
        if (from == null)
        {
            _logger.LogWarning("Получено сообщение без отправителя (From is null). MessageId: {Id}", message.Id);
            return;
        }

        if (!_allowedUsers.Contains(from.Id))
        {
            _logger.LogWarning("Unauthorized access attempt from ID: {UserId}", from.Id);
            await botClient.SendMessage(message.Chat.Id, "У вас нет доступа к этому боту.", cancellationToken: ct);
            return;
        }

        try
        {
            var (user, isNew) = await _userService.RegisterUserAsync(from.Id, from.FirstName, from.Username, ct);

            _logger.LogInformation("Обработка сообщения от {UserId}: {Text}", user.TelegramId, text);

            await _commandExecutor.ExecuteCommand(text, botClient, message, user, ct);
        }
        catch (ArgumentException ex)
        {
            _logger.LogError(ex, "Error handling update, argument exception");
            await botClient.SendMessage(message.Chat.Id, $"❌ {ex.Message}", cancellationToken: ct);
        }
    }
}