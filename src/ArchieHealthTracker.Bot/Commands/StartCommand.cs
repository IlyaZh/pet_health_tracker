using ArchieHealthTracker.Application.Services;
using ArchieHealthTracker.Bot.Bot;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ArchieHealthTracker.Bot.Commands;

public class StartCommand(IUserSessionService userSessionService) : ITelegramCommand
{
    public string CommandName { get; } = "/start";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user, CancellationToken ct)
    {
        userSessionService.ClearSession(user.Id);
        var text = message.Text == "/start"
            ? $"Привет, {user.FirstName}! 👋\n\n" +
              "Я помогу тебе следить за здоровьем **Арчи**. " +
              "Выбери нужное действие в меню внизу или используй команды."
            : "Главное меню:";

        await botClient.SendMessage(
            message.Chat.Id,
            text,
            ParseMode.Markdown,
            replyMarkup: BotNavigation.Keyboards.Main,
            cancellationToken: ct
        );
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, UserSession session, Message message, BotUser user,
        string text,
        CancellationToken ct)
    {
        await Task.CompletedTask;
    }
}
