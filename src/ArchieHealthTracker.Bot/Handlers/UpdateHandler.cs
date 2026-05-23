using ArchieHealthTracker.Application.Services;
using ArchieHealthTracker.Bot.Configuration;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Handlers;

public class UpdateHandler(
    IUserService userService,
    CommandExecutor commandExecutor,
    ILogger<UpdateHandler> logger,
    IOptions<BotConfiguration> botConfiguration,
    IUserSessionService userSessionService)
{
    private readonly HashSet<long> _allowedUsers = botConfiguration.Value.AllowedUsers;

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
            logger.LogWarning("Получено сообщение без отправителя (From is null). MessageId: {Id}", message.Id);
            return;
        }

        if (!_allowedUsers.Contains(from.Id))
        {
            logger.LogWarning("Unauthorized access attempt from ID: {UserId}", from.Id);
            await botClient.SendMessage(message.Chat.Id, "У вас нет доступа к этому боту.", cancellationToken: ct);
            return;
        }

        BotUser? requestedUser = null;
        try
        {
            var (user, isNew) = await userService.RegisterUserAsync(from.Id, from.FirstName, from.Username, ct);
            requestedUser = user;

            logger.LogInformation("Обработка сообщения от {UserId} ({Username})", user.TelegramId,
                from.Username);

            await commandExecutor.ExecuteCommand(text, botClient, message, user, ct);
        }
        catch (ArgumentException ex)
        {
            if (requestedUser != null)
            {
                userSessionService.ClearSession(requestedUser.Id);
            }

            logger.LogError(ex, "Error handling update, argument exception");
            await botClient.SendMessage(message.Chat.Id, $"❌ {ex.Message}", cancellationToken: ct);
        }
    }
}