using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Extensions;
using ArchieHealthTracker.Repositories;
using ArchieHealthTracker.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ArchieHealthTracker.Bot.Commands;

public class HygieneCommand : ITelegramCommand
{
    private readonly IUserSessionService _userSessionService;
    private readonly IHealthService _healthService;
    public string CommandName { get; } = "/hygiene";

    private readonly string _chooseVariant = "Выбери процедуру для Арчи:";

    public HygieneCommand(IUserSessionService userSessionService,  IHealthService healthService)
    {
        _userSessionService = userSessionService;
        _healthService = healthService;
    }

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user,
        CancellationToken ct)
    {
        var actions = Enum.GetValues<HygieneEventType>()
            .Where(t => t != HygieneEventType.Unknown);

        var rows = actions
            .Select(type => InlineKeyboardButton.WithCallbackData(type.GetDescription(), $"hygiene:{(int)type}"))
            .Chunk(3); 
        var keyboard = new InlineKeyboardMarkup(rows);

        var sentMessage = await botClient.SendMessage(
            message.Chat.Id,
            _chooseVariant,
            replyMarkup: keyboard,
            cancellationToken: ct
        );
        _userSessionService.SetUserState(user.Id, new UserSession
        {
            CommandName = CommandName,
            MessageId = sentMessage.Id,
        });
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, UserSession session, Message message,
        BotUser user,
        CancellationToken ct)
    {
        var text = message.Text;
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var parts = text.Split(':');
        if (parts.Length < 2 || !Enum.TryParse<HygieneEventType>(parts[1], out var type))
        {
            throw new ArgumentException($"Invalid hygiene input");
        }

        await _healthService.AddHygiene(user, type, ct);
        var typeName = type.GetDescription();

        await botClient.EditMessageText(
            message.Chat.Id,
            session.MessageId,
            $"✅ Записал: {typeName} для Арчи!",
            cancellationToken: ct
        );

        _userSessionService.ClearSession(user.Id);
    }
}