using System.Globalization;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;
using ArchieHealthTracker.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Commands;

public class WeightCommand : ITelegramCommand
{
    private readonly IHealthService _healthService;
    private readonly IUserSessionService _userSessionService;

    public WeightCommand(IHealthService healthService,  IUserSessionService userSessionService)
    {
        _healthService = healthService;
        _userSessionService = userSessionService;
    }

    public string CommandName { get; } = "weight";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user,
        CancellationToken cancellationToken)
    {
         await botClient.SendMessage(message.Chat.Id, "Пришлите вес в формате 7.5 (в килограммах)",
            cancellationToken: cancellationToken);
          _userSessionService.SetCommandState(user.Id, CommandName);
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, Message message, BotUser user,
        CancellationToken ct)
    {
        var chatId = message.Chat.Id;
        var text = message.Text?.Replace(',', '.');
        var isParsed = !double.TryParse(text, CultureInfo.InvariantCulture, out var weight); 
        if (!isParsed)
        {
            await botClient.SendMessage(chatId, "Это не похоже на число. Попробуй еще раз или нажми /cancel", cancellationToken: ct);
            return;
        }
        
        await _healthService.AddWeight(user, Weight.FromKilograms(weight));
        _userSessionService.ClearSession(user.Id);
        await botClient.SendMessage(chatId, $"✅ Вес {weight} кг сохранен!", cancellationToken: ct);
        
    }
}