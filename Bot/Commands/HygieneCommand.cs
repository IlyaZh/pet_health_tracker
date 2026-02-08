using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Commands;

public class HygieneCommand : ITelegramCommand
{
    public string CommandName { get; } = "hygiene";
    
    public Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task HandleInputAsync(ITelegramBotClient botClient, Message message, BotUser user, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}