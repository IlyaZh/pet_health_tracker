using ArchieHealthTracker.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Interfaces;

public interface ITelegramCommand
{
    string CommandName { get; }

    Task ExecuteAsync(
        ITelegramBotClient botClient,
        Message message,
        BotUser user,
        CancellationToken ct
    );

    Task HandleInputAsync(
        ITelegramBotClient botClient,
        UserSession session,
        Message message,
        BotUser user,
        string text,
        CancellationToken ct
    );
}