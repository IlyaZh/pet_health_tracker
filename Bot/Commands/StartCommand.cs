using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Bot.Keyboards;
using ArchieHealthTracker.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Commands;

public class StartCommand : ITelegramCommand
{
    public string CommandName { get; } = "/start";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user, CancellationToken ct)
    {
        var welcomeText =
            $"Привет, {user.FirstName}! 👋\n\n" +
            "Я помогу тебе следить за здоровьем **Арчи**. " +
            "Выбери нужное действие в меню внизу или используй команды.";

        await botClient.SendMessage(
            message.Chat.Id,
            welcomeText,
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            replyMarkup: MainMenuKeyboard.Get(),
            cancellationToken: ct
        );
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, UserSession session, Message message, BotUser user,
        CancellationToken ct)
    {
        await Task.CompletedTask;
    }
}