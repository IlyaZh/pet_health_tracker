using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Extensions;
using ArchieHealthTracker.Repositories;
using ArchieHealthTracker.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ArchieHealthTracker.Bot.Commands;

public class SymptomCommand : ITelegramCommand
{
    public string CommandName { get; } = "/symptom";
    private readonly IUserSessionService _userSessionService;
    private readonly IHealthService _healthService;
    private readonly ILogger<SymptomCommand> _logger;

    private readonly string _whatHappened = "Что случилось с Арчи?";

    public SymptomCommand(
        IUserSessionService userSessionService,
        IHealthService healthService,
        ILogger<SymptomCommand> logger
        )
    {
        _userSessionService = userSessionService;
        _healthService = healthService;
        _logger = logger;
    }

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user,
        CancellationToken ct)
    {
        var buttons = Enum.GetValues<SymptomType>()
            .Where(t => t != SymptomType.Unknown)
            .Select(t => InlineKeyboardButton.WithCallbackData(t.GetDescription(), $"symptom:{(int)t}"))
            .Chunk(3);

        var sentMessage = await botClient.SendMessage(
            message.Chat.Id,
            _whatHappened,
            replyMarkup: new InlineKeyboardMarkup(buttons),
            cancellationToken: ct
        );

        _userSessionService.SetUserState(user.Id, new UserSession
        {
            CommandName = CommandName,
            MessageId = sentMessage.Id,
        });
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, UserSession session, Message message, BotUser user,
        string text,
        CancellationToken ct)
    {
        var input = message.Text;
        _logger.LogInformation($"[SymptomCommand] Text: {text}");
            
        if (!string.IsNullOrEmpty(input) && input.StartsWith("symptom:"))
        {
            var parts = input.Split(':');
            if (parts.Length < 2 || !Enum.TryParse<SymptomType>(parts[1], out var type))
            {
                throw new ArgumentException("Invalid symptom input");
            }

            var typeId = parts[1];
            _logger.LogInformation($"[SymptomCommand] Type: {typeId}");

            session.Metadata["type"] = typeId;
            session.Metadata["step"] = "awaiting_details";
            _userSessionService.SetUserState(user.Id, session);
            
            await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                $"Выбран симптом: *{type.GetDescription()}*\n\nНапиши подробности (например, 'после прогулки') или нажми кнопку 'Пропустить':",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                replyMarkup: new InlineKeyboardMarkup(InlineKeyboardButton.WithCallbackData("⏩ Пропустить", "symptom:skip")),
                cancellationToken: ct);
            return;
        }

        if (session.Metadata.TryGetValue("type", out var storedTypeId))
        {
            var type = (SymptomType)int.Parse(storedTypeId);
            var note = (input == "symptom:skip") ? null : input;
            var symptom = new Symptom
            {
                Type = type,
                Note = note
            };
            await _healthService.AddSymptomAsync(user, symptom, ct);
            
            await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                $"✅ Записал симптом: *{type.GetDescription()}*\nДетали: _{note ?? "нет"}_",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                cancellationToken: ct);

            _userSessionService.ClearSession(user.Id);
        }
    }
}