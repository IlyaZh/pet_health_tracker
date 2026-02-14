using ArchieHealthTracker.Bot.Helpers;
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

public class MedicalEventCommand : ITelegramCommand
{
    private readonly IUserSessionService _userSessionService;
    private readonly IHealthService _healthService;
    private readonly ILogger<MedicalEventCommand> _logger;

    private readonly string _chooseVariant = "Выбери тип события:";
    private readonly string _cancelButtonLabel = "❌ Отменить";
    private readonly string _cancelButtonCallback = "medical_event:cancel";

    public string CommandName { get; } = "/medical_event";

    public MedicalEventCommand(
        IUserSessionService userSessionService,
        IHealthService healthService,
        ILogger<MedicalEventCommand> logger
    )
    {
        _userSessionService = userSessionService;
        _healthService = healthService;
        _logger = logger;
    }

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user, CancellationToken ct)
    {
        var actions = Enum.GetValues<MedicalEventType>()
            .Where(t => t != MedicalEventType.Unknown);
        var rows = actions
            .Select(type =>
                InlineKeyboardButton.WithCallbackData(type.GetDescription(), $"medical_event:{type.ToString()}"))
            .Chunk(3);
        var keyboard = new InlineKeyboardMarkup(rows);

        var sentMessage = await botClient.SendMessage(
            message.Chat.Id,
            _chooseVariant,
            replyMarkup: keyboard,
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
        if (string.IsNullOrEmpty(input))
        {
            throw new ArgumentException("Invalid input");
        }

        _logger.LogDebug($"[MedicalEventCommand] message: {input}");

        if (input.StartsWith("medical_event:"))
        {
            var parts = input.Split(':');
            if (parts.Length < 2)
            {
                throw new ArgumentException("Invalid medical_event input, not enough arguments");
            }

            var arg = parts[1];
            if (arg == "cancel")
            {
                await botClient.EditMessageText(
                    message.Chat.Id,
                    session.MessageId,
                    "❌ Ввод медицинского события отменен.",
                    cancellationToken: ct
                );
                _userSessionService.ClearSession(user.Id);

                await botClient.SendMessage(message.Chat.Id, "Чем еще могу помочь?",
                    replyMarkup: BotNavigation.Keyboards.Main, cancellationToken: ct);
                return;
            }

            if (Enum.TryParse<MedicalEventType>(parts[1], out var type))
            {
                var typeStr = arg;
                session.Metadata["type"] = typeStr;
                session.Metadata["step"] = "title";
                _userSessionService.SetUserState(user.Id, session);

                await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                    $"Выбрано событие: *{type.GetDescription()}*\n\nНапиши наименование события (например, 'Посещение Клиники' или 'Бравекто') или нажми кнопку 'Пропустить':",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetCancelKeyboard(),
                    cancellationToken: ct);
                return;
            }
        }

        var currentStep = session.Metadata.GetValueOrDefault("step");
        switch (currentStep)
        {
            case "title":
                session.Metadata["title"] = input;
                session.Metadata["step"] = "dosage";
                _userSessionService.SetUserState(user.Id, session);

                await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                    $"Название: *{input}*\n\n💊 Введи дозировку (или нажми пропустить):",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetSkipAndCancelKeyboard("dosage"),
                    cancellationToken: ct);
                break;
            case "dosage":
                session.Metadata["dosage"] = input == "skip:dosage" ? "не указана" : input;
                session.Metadata["step"] = "note";
                _userSessionService.SetUserState(user.Id, session);

                await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                    $"Дозировка: *{session.Metadata["dosage"]}*\n\n🗒 Добавь заметку (или пропусти):",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetSkipAndCancelKeyboard("note"),
                    cancellationToken: ct);
                break;
            case "note":
                var note = input == "skip:note" ? null : input;
                var isParsed = Enum.TryParse<MedicalEventType>(session.Metadata["type"].AsSpan(), out var finalType);
                if (!isParsed)
                {
                    throw new ArgumentException("Invalid medical_event input, not enough arguments");
                }

                await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                    $"✅ *Запись сохранена!*\n" +
                    $"Тип: {finalType.GetDescription()}\n" +
                    $"Название: {session.Metadata["title"]}\n" +
                    $"Дозировка: {session.Metadata["dosage"]}\n" +
                    $"Заметка: {note ?? "-"}",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    cancellationToken: ct);

                var medicalEvent = new MedicalEvent
                {
                    Type = finalType,
                    Title = session.Metadata["title"],
                    Dosage = session.Metadata["dosage"],
                    Note = session.Metadata["note"]
                };
                await _healthService.AddMedicalEvent(user, medicalEvent, ct);

                _userSessionService.ClearSession(user.Id);
                break;
        }
    }

    private InlineKeyboardMarkup GetCancelKeyboard() =>
        new(InlineKeyboardButton.WithCallbackData(_cancelButtonLabel, _cancelButtonCallback));

    private InlineKeyboardMarkup GetSkipAndCancelKeyboard(string step) => new(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("⏩ Пропустить", $"skip:{step}") },
        new[] { InlineKeyboardButton.WithCallbackData(_cancelButtonLabel, _cancelButtonCallback) }
    });
}