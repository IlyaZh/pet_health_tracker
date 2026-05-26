using ArchieHealthTracker.Application.Services;
using ArchieHealthTracker.Bot.Bot;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Extensions;
using ArchieHealthTracker.Domain.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ArchieHealthTracker.Bot.Commands;

public class HistoryCommand(
    IReportQueue reportQueue,
    IHealthService healthService,
    IUserSessionService userSessionService,
    ILogger<HistoryCommand> logger)
    : ITelegramCommand
{
    private readonly string _cancelButtonCallback = "report_type:cancel";

    private readonly string _chooseVariant = "Выбери тип отчета:";
    private readonly IHealthService _healthService = healthService;

    private readonly Dictionary<string, int> _periods = new()
    {
        ["1 месяц"] = 1,
        ["3 месяца"] = 3,
        ["6 месяцев"] = 6,
        ["1 год"] = 12,
        ["2 года"] = 24
    };

    private readonly int TelegramMaxRows = 20;

    public string CommandName { get; } = "/history";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user, CancellationToken ct)
    {
        var actions = Enum.GetValues<ReportCategory>()
            .Where(t => t != ReportCategory.Unknown);
        var rows = actions
            .Select(type =>
                InlineKeyboardButton.WithCallbackData(type.GetDescription(), $"report_type:{type.ToString()}"))
            .Chunk(3);
        var keyboard = new InlineKeyboardMarkup(rows);


        var sentMessage = await botClient.SendMessage(
            message.Chat.Id,
            _chooseVariant,
            ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: ct
        );

        var session = new UserSession
        {
            CommandName = CommandName,
            MessageId = sentMessage.Id
        };
        userSessionService.SetUserState(user.Id, session);
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, UserSession session, Message message, BotUser user,
        string text, CancellationToken ct)
    {
        logger.LogInformation("[HistoryCommand] HandleInput: {Text} at Step: {Step}", text,
            session.Metadata.GetValueOrDefault("step", "Start"));

        if (text == _cancelButtonCallback)
        {
            await CancelFlowAsync(botClient, session, user, message, ct);
            return;
        }

        // 1. Обработка ВЫБОРА ТИПА (первый шаг)
        if (text.StartsWith("report_type:"))
        {
            var type = text.Split(':')[1];
            session.Metadata["type"] = type;
            session.Metadata["step"] = nameof(ReportStep.Period);
            userSessionService.SetUserState(user.Id, session);

            await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                $"📊 Тип: *{Enum.Parse<ReportCategory>(type).GetDescription()}*\n\n📅 Выберите период:",
                ParseMode.Markdown, GetPeriodKeyboard(), cancellationToken: ct);
            return;
        }

        // 2. Обработка ПЕРИОДА
        if (text.StartsWith("report_period:"))
        {
            var period = text.Split(':')[1];
            session.Metadata["period"] = period;
            session.Metadata["step"] = nameof(ReportStep.Format);
            userSessionService.SetUserState(user.Id, session);

            var periodText = _periods.FirstOrDefault(x => x.Value.ToString() == period).Key;
            await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                $"📅 Период: *{periodText}*\n\nОтправить как текст в чат или подготовить PDF?",
                ParseMode.Markdown, GetFormatKeyboard(), cancellationToken: ct);
            return;
        }

        // 3. Обработка ФОРМАТА (финал)
        if (text.StartsWith("report_format:"))
        {
            var format = text.Split(':')[1];
            session.Metadata["format"] = format;
            await FinishAsync(botClient, message, user, session, ct);
        }
    }

    private async Task CancelFlowAsync(
        ITelegramBotClient botClient,
        UserSession session,
        BotUser user,
        Message message,
        CancellationToken ct
    )
    {
        await botClient.EditMessageText(
            message.Chat.Id,
            session.MessageId,
            "❌ Ввод отменен.",
            cancellationToken: ct
        );
        userSessionService.ClearSession(user.Id);

        await botClient.SendMessage(
            message.Chat.Id,
            "Чем еще могу помочь?",
            replyMarkup: BotNavigation.Keyboards.Main,
            cancellationToken: ct
        );
    }

    private async Task FinishAsync(ITelegramBotClient botClient, Message message, BotUser user, UserSession session,
        CancellationToken ct)
    {
        var category = Enum.Parse<ReportCategory>(session.Metadata["type"]);
        var periodMonths = int.Parse(session.Metadata["period"]);
        var format = Enum.Parse<ReportFormat>(session.Metadata["format"]);

        var from = DateTime.UtcNow.AddMonths(-periodMonths);

        int? limit = format == ReportFormat.Telegram ? TelegramMaxRows : null;

        var reportRequest = new ReportRequest(
            user.TelegramId,
            category,
            DateFrom: from,
            Limit: limit
        );

        await reportQueue.EnqueueReportAsync(new ReportQueueItem(
            Format: format,
            Request: reportRequest,
            ChatId: message.Chat.Id
        ));

        var confirmationText = format == ReportFormat.Pdf
            ? "✅ *Заявка на PDF принята.*\nГенерация файла может занять несколько секунд."
            : "✅ *Заявка принята.*\nСейчас пришлю последние данные текстом.";

        await botClient.EditMessageText(
            message.Chat.Id,
            session.MessageId,
            confirmationText,
            ParseMode.Markdown,
            cancellationToken: ct
        );

        userSessionService.ClearSession(user.Id);
    }

    private InlineKeyboardMarkup GetPeriodKeyboard()
    {
        var rows = _periods
            .Select(p =>
                InlineKeyboardButton.WithCallbackData(p.Key, $"report_period:{p.Value.ToString()}"))
            .Chunk(3);

        return new InlineKeyboardMarkup(rows);
    }

    private InlineKeyboardMarkup GetFormatKeyboard()
    {
        return new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("📱 Текст", "report_format:Telegram"),
                InlineKeyboardButton.WithCallbackData("📄 PDF файл", "report_format:Pdf")
            },
            new[] { InlineKeyboardButton.WithCallbackData("❌ Отмена", _cancelButtonCallback) }
        });
    }
}
