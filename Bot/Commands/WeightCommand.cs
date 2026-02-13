using System.Globalization;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;
using ArchieHealthTracker.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Commands;

public class WeightCommand : ITelegramCommand
{
    private readonly IHealthService _healthService;
    private readonly IUserSessionService _userSessionService;
    private readonly ILogger<WeightCommand> _logger;

    public WeightCommand(
        IHealthService healthService,
        IUserSessionService userSessionService,
        ILogger<WeightCommand> logger
    )
    {
        _healthService = healthService;
        _userSessionService = userSessionService;
        _logger = logger;
    }

    public string CommandName { get; } = "/weight";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user,
        CancellationToken ct)
    {
        var sentMessage = await botClient.SendMessage(message.Chat.Id, "Пришлите вес в формате 7.5 (в килограммах)",
            cancellationToken: ct);
        _userSessionService.SetUserState(user.Id, new UserSession
        {
            CommandName = CommandName,
            MessageId = sentMessage.MessageId,
        });
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, UserSession session, Message message,
        BotUser user,
        CancellationToken ct)
    {
        var chatId = message.Chat.Id;
        var text = message.Text?.Replace(',', '.');
        _logger.LogInformation("[WeightCommand] Message: {text}", text);
        _logger.LogInformation("[WeightCommand] User: {user}", user);
        var isParsed = double.TryParse(text, CultureInfo.InvariantCulture, out var weight);
        if (!isParsed)
        {
            await botClient.SendMessage(chatId, "Это не похоже на число. Попробуй еще раз или нажми /cancel",
                cancellationToken: ct);
            return;
        }

        await _healthService.AddWeight(user, Weight.FromKilograms(weight), ct);
        _userSessionService.ClearSession(user.Id);
        await botClient.EditMessageText(chatId, session.MessageId, $"✅ Вес {weight} кг сохранен!",
            cancellationToken: ct);
    }
}