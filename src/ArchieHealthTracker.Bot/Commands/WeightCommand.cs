using System.Globalization;
using ArchieHealthTracker.Application.Services;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Commands;

public class WeightCommand(
    IHealthService healthService,
    IUserSessionService userSessionService,
    ILogger<WeightCommand> logger)
    : ITelegramCommand
{
    public string CommandName { get; } = "/weight";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user,
        CancellationToken ct)
    {
        var sentMessage = await botClient.SendMessage(message.Chat.Id, "Пришлите вес в формате 7.5 (в килограммах)",
            cancellationToken: ct);
        userSessionService.SetUserState(user.Id, new UserSession
        {
            CommandName = CommandName,
            MessageId = sentMessage.MessageId
        });
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, UserSession session, Message message,
        BotUser user,
        string text,
        CancellationToken ct)
    {
        var chatId = message.Chat.Id;
        text = text.Replace(',', '.');
        logger.LogInformation("[WeightCommand] Processing weight input for user {UserId}", user.Id);
        var isParsed = double.TryParse(text, CultureInfo.InvariantCulture, out var weight);
        if (!isParsed)
        {
            await botClient.SendMessage(chatId, "Это не похоже на число. Попробуй еще раз или нажми /cancel",
                cancellationToken: ct);
            return;
        }

        await healthService.AddWeightAsync(user, Weight.FromKilograms(weight), ct);
        userSessionService.ClearSession(user.Id);
        await botClient.EditMessageText(chatId, session.MessageId, $"✅ Вес {weight} кг сохранен!",
            cancellationToken: ct);
    }
}
