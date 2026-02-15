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

    private async Task Finish(
        ITelegramBotClient botClient,
        Message message,
        BotUser user,
        UserSession session,
        CancellationToken ct
    )
    {
        var isParsed = Enum.TryParse<MedicalEventType>(session.Metadata["type"].AsSpan(), out var finalType);
        if (!isParsed)
        {
            throw new ArgumentException("Invalid medical_event input, not enough arguments");
        }

        session.Metadata.TryGetValue("dosage", out var dosage);
        session.Metadata.TryGetValue("note", out var note);

        await botClient.EditMessageText(message.Chat.Id, session.MessageId,
            $"✅ *Запись сохранена!*\n" +
            $"Тип: {finalType.GetDescription()}\n" +
            $"Название: {session.Metadata["title"]}\n" +
            $"Дозировка: {dosage ?? "-"}\n" +
            $"Заметка: {note ?? "-"}",
            parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
            cancellationToken: ct);
        var medicalEvent = new MedicalEvent
        {
            Type = finalType,
            Title = session.Metadata["title"],
            Dosage = dosage,
            Note = note,
        };
        await _healthService.AddMedicalEvent(user, medicalEvent, ct);

        _userSessionService.ClearSession(user.Id);
    }

    public async Task HandleInputAsync(
        ITelegramBotClient botClient,
        UserSession session,
        Message message,
        BotUser user,
        string text,
        CancellationToken ct)
    {
        _logger.LogInformation("[MedicalEventCommand] HandleInputAsync");
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Invalid input");
        }

        _logger.LogInformation($"[MedicalEventCommand] message: {text}");

        if (text.StartsWith("medical_event:"))
        {
            var parts = text.Split(':');
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
                session.Metadata["title"] = text;
                session.Metadata["step"] = "dosage";
                _userSessionService.SetUserState(user.Id, session);

                await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                    $"Название: *{text}*\n\n💊 Введи дозировку (или нажми пропустить):",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetSkipAndCancelKeyboard("dosage"),
                    cancellationToken: ct);
                break;
            case "dosage":
                if (text != "skip:dosage" )
                {
                    session.Metadata["dosage"] = text;
                }

                session.Metadata["step"] = "note";
                _userSessionService.SetUserState(user.Id, session);
                var dosageMessage = session.Metadata.GetValueOrDefault("dosage", "не указано"); 

                await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                    $"Дозировка: *{dosageMessage}*\n\n🗒 Добавь заметку (или пропусти):",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetSkipAndCancelKeyboard("note"),
                    cancellationToken: ct);
                break;
            case "note":
                if (text != "skip:note")
                {
                    session.Metadata["note"] = text;
                }

                _userSessionService.SetUserState(user.Id, session);
                await Finish(botClient, message, user, session, ct);

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