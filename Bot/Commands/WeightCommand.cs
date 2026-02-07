using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Commands;

public class WeightCommand : ITelegramCommand
{
    private readonly IHealthService _healthService;

    public  WeightCommand(IHealthService healthService)
    {
        _healthService = healthService;
    }

    public string CommandName { get; } = "weight";

    public Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user,
        CancellationToken cancellationToken)
    {
        // tbd
        return botClient.SendMessage(message.Chat.Id, "Пришлите вес в формате 7.5 (в килограммах)",
            cancellationToken: cancellationToken);
    }
}