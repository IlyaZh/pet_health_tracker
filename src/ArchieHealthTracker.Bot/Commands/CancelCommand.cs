using ArchieHealthTracker.Application.Services;
using ArchieHealthTracker.Bot.Bot;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Commands;

public class CancelCommand(IUserSessionService userSessionService) : ITelegramCommand
{
    public string CommandName { get; } = "/cancel";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user, CancellationToken ct)
    {
        userSessionService.ClearSession(user.Id);

        await botClient.SendMessage(
            message.Chat.Id,
            "❌ Действие отменено. Чем еще могу помочь?",
            replyMarkup: BotNavigation.Keyboards.Main,
            cancellationToken: ct
        );
    }

    public Task HandleInputAsync(
        ITelegramBotClient botClient,
        UserSession session,
        Message message,
        BotUser user,
        string text,
        CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}
