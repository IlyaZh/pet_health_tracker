using ArchieHealthTracker.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Interfaces;

public interface ITelegramCommand
{
    string CommandName { get; }
    Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user, CancellationToken cancellationToken);
}