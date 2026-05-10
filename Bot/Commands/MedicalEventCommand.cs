using ArchieHealthTracker.Bot.Helpers;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Extensions;
using ArchieHealthTracker.Flows;
using ArchieHealthTracker.Domain.Repositories;
using ArchieHealthTracker.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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

    private async Task FinishAsync(
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

        session.Metadata.TryGetValue(nameof(MedicalEventStep.Dosage), out var dosage);
        session.Metadata.TryGetValue(nameof(MedicalEventStep.Note), out var note);
        session.Metadata.TryGetValue(nameof(MedicalEventStep.Title), out var title);

        await botClient.EditMessageText(
            message.Chat.Id,
            session.MessageId,
            $"✅ *Запись сохранена!*\n" +
            $"Тип: {finalType.GetDescription()}\n" +
            $"Название: {title}\n" +
            $"Дозировка: {dosage ?? "-"}\n" +
            $"Заметка: {note ?? "-"}",
            parseMode: ParseMode.Markdown,
            cancellationToken: ct);

        var medicalEvent = new MedicalEvent
        {
            Type = finalType,
            Title = title ?? "Без названия",
            Dosage = dosage,
            Note = note,
        };
        await _healthService.AddMedicalEventAsync(user, medicalEvent, ct);

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
        if (string.IsNullOrEmpty(text))
        {
            throw new ArgumentException("Invalid input");
        }

        if (text == _cancelButtonCallback)
        {
            await botClient.EditMessageText(
                message.Chat.Id,
                session.MessageId,
                "❌ Ввод отменен.",
                cancellationToken: ct
            );
            _userSessionService.ClearSession(user.Id);

            await botClient.SendMessage(message.Chat.Id, "Чем еще могу помочь?",
                replyMarkup: BotNavigation.Keyboards.Main, cancellationToken: ct);
            return;
        }


        if (!session.Metadata.ContainsKey("step") && text.StartsWith("medical_event:"))
        {
            var typeStr = text.Split(':')[1];
            if (Enum.TryParse<MedicalEventType>(typeStr, out var type))
            {
                session.Metadata["type"] = type.ToString();
                session.Metadata["step"] = MedicalEventStep.Title.ToString();
                _userSessionService.SetUserState(user.Id, session);

                await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                    $"Выбрано: *{type.GetDescription()}*\n\nВведите название:",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetCancelKeyboard(), cancellationToken: ct);
                return;
            }
        }

        var currentStepStr = session.Metadata["step"];
        var currentStep = Enum.Parse<MedicalEventStep>(currentStepStr);
        var eventType = Enum.Parse<MedicalEventType>(session.Metadata["type"]);

        if (!text.StartsWith("skip:"))
        {
            session.Metadata[currentStep.ToString()] = text;
        }
        else
        {
            session.Metadata[currentStep.ToString()] = "";
        }
        _userSessionService.SetUserState(user.Id, session);

        var nextStep = MedicalEventFlowConfig.GetNextStep(eventType, currentStep);

        if (nextStep.HasValue)
        {
            session.Metadata["step"] = nextStep.Value.ToString();
            _userSessionService.SetUserState(user.Id, session);

            await AskNextStepAsync(botClient, message, session, nextStep.Value, ct);
            return;
        }

        await FinishAsync(botClient, message, user, session, ct);
    }

    private async Task AskNextStepAsync(
        ITelegramBotClient bot,
        Message msg,
        UserSession session,
        MedicalEventStep step,
        CancellationToken ct
    )
    {
        var prompt = step switch
        {
            MedicalEventStep.Dosage => "💊 Введите дозировку:",
            MedicalEventStep.Note => "🗒 Добавьте заметку:",
            _ => "Введите данные:"
        };

        await bot.EditMessageText(
            msg.Chat.Id,
            session.MessageId,
            prompt,
            replyMarkup: GetSkipAndCancelKeyboard(step.ToString().ToLower()),
            cancellationToken: ct
        );
    }


    private InlineKeyboardMarkup GetCancelKeyboard() =>
        new(InlineKeyboardButton.WithCallbackData(_cancelButtonLabel, _cancelButtonCallback));

    private InlineKeyboardMarkup GetSkipAndCancelKeyboard(string step) => new(new[]
    {
        new[] { InlineKeyboardButton.WithCallbackData("⏩ Пропустить", $"skip:{step}") },
        new[] { InlineKeyboardButton.WithCallbackData(_cancelButtonLabel, _cancelButtonCallback) }
    });
}