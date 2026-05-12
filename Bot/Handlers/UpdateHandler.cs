using ArchieHealthTracker.Configuration;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Repositories;
using ArchieHealthTracker.Services;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Handlers;

public class UpdateHandler
{
    private readonly HashSet<long> _allowedUsers;
    private readonly CommandExecutor _commandExecutor;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly IUserService _userService;
    private readonly IUserSessionService _userSessionService;

    public UpdateHandler(
        IUserService userService,
        CommandExecutor commandExecutor,
        ILogger<UpdateHandler> logger,
        IOptions<BotConfiguration> botConfiguration,
        IUserSessionService userSessionService
    )
    {
        _userService = userService;
        _commandExecutor = commandExecutor;
        _logger = logger;
        _allowedUsers = botConfiguration.Value.AllowedUsers;
        _userSessionService = userSessionService;
    }

    public async Task HandlerAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        if (update.Message is { Text: { } messageText } message)
        {
            await HandleUpdateAsync(botClient, message, message.From, messageText, ct);
        }
        else if (update.CallbackQuery is { Data: { } callbackData } callback)
        {
            await HandleUpdateAsync(botClient, callback.Message!, callback.From, callbackData, ct);
            await botClient.AnswerCallbackQuery(callback.Id, cancellationToken: ct);
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Message message, User? from, string text,
        CancellationToken ct)
    {
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

        BotUser? requestedUser = null;
        try
        {
            var (user, isNew) = await _userService.RegisterUserAsync(from.Id, from.FirstName, from.Username, ct);
            requestedUser = user;

            _logger.LogInformation("Обработка сообщения от {UserId} ({Username}): {Text}", user.TelegramId,
                from.Username, text);

            await _commandExecutor.ExecuteCommand(text, botClient, message, user, ct);
        }
        catch (ArgumentException ex)
        {
            if (requestedUser != null)
            {
                _userSessionService.ClearSession(requestedUser.Id);
            }

            _logger.LogError(ex, "Error handling update, argument exception");
            await botClient.SendMessage(message.Chat.Id, $"❌ {ex.Message}", cancellationToken: ct);
        }
    }
}