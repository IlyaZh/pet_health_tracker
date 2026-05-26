using ArchieHealthTracker.Application.Services;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Extensions;
using ArchieHealthTracker.Domain.Interfaces.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ArchieHealthTracker.Bot.Commands;

public class HygieneCommand(IUserSessionService userSessionService, IHealthService healthService)
    : ITelegramCommand
{
    private readonly string _chooseVariant = "Выбери процедуру для Арчи:";

    public string CommandName { get; } = "/hygiene";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user,
        CancellationToken ct)
    {
        var actions = Enum.GetValues<HygieneEventType>()
            .Where(t => t != HygieneEventType.Unknown);

        var rows = actions
            .Select(type => InlineKeyboardButton.WithCallbackData(type.GetDescription(), $"hygiene:{type.ToString()}"))
            .Chunk(3);
        var keyboard = new InlineKeyboardMarkup(rows);

        var sentMessage = await botClient.SendMessage(
            message.Chat.Id,
            _chooseVariant,
            replyMarkup: keyboard,
            cancellationToken: ct
        );
        userSessionService.SetUserState(user.Id, new UserSession
        {
            CommandName = CommandName,
            MessageId = sentMessage.Id
        });
    }

    public async Task HandleInputAsync(
        ITelegramBotClient botClient,
        UserSession session,
        Message message,
        BotUser user,
        string text,
        CancellationToken ct)
    {
        if (string.IsNullOrEmpty(text)) return;

        var parts = text.Split(':');
        if (parts.Length < 2 || !Enum.TryParse<HygieneEventType>(parts[1], out var type))
            throw new ArgumentException("Invalid hygiene input");

        await healthService.AddHygieneAsync(user, type, ct);
        var typeName = type.GetDescription();

        await botClient.EditMessageText(
            message.Chat.Id,
            session.MessageId,
            $"✅ Записал: {typeName} для Арчи!",
            cancellationToken: ct
        );

        userSessionService.ClearSession(user.Id);
    }
}