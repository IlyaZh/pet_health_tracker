using ArchieHealthTracker.Bot.Helpers;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Commands;

public class StartCommand : ITelegramCommand
{
    private readonly IUserSessionService _userSessionService;
    public string CommandName { get; } = "/start";

    public StartCommand(IUserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
    }

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user, CancellationToken ct)
    {
        _userSessionService.ClearSession(user.Id);
        var text = message.Text == "/start"
            ? $"Привет, {user.FirstName}! 👋\n\n" +
              "Я помогу тебе следить за здоровьем **Арчи**. " +
              "Выбери нужное действие в меню внизу или используй команды."
            : "Главное меню:";

        await botClient.SendMessage(
            message.Chat.Id,
            text,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: BotNavigation.Keyboards.Main,
            cancellationToken: ct
        );
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, UserSession session, Message message, BotUser user,
        CancellationToken ct)
    {
        await Task.CompletedTask;
    }
}