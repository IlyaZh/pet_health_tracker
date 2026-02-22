using ArchieHealthTracker.Bot.Helpers;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Extensions;
using ArchieHealthTracker.Flows;
using ArchieHealthTracker.Repositories;
using ArchieHealthTracker.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ArchieHealthTracker.Bot.Commands;

public class HistoryCommand : ITelegramCommand
{
    private readonly IReportQueue _reportQueue;
    private readonly IHealthService _healthService;
    private readonly IUserSessionService _userSessionService;
    private readonly ILogger<HistoryCommand> _logger;

    private readonly string _chooseVariant = "Выбери тип отчета:";
    private readonly string _cancelButtonCallback = "report_type:cancel";

    private readonly Dictionary<string, int> _periods = new()
    {
        ["1 месяц"] = 1,
        ["3 месяца"] = 3,
        ["6 месяцев"] = 6,
        ["1 год"] = 12,
        ["2 года"] = 24
    };

    public string CommandName { get; } = "/history";

    public HistoryCommand(
        IReportQueue reportQueue,
        IHealthService healthService,
        IUserSessionService userSessionService,
        ILogger<HistoryCommand> logger
    )
    {
        _reportQueue = reportQueue;
        _healthService = healthService;
        _userSessionService = userSessionService;
        _logger = logger;
    }

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
            parseMode: ParseMode.Markdown,
            replyMarkup: keyboard,
            cancellationToken: ct
        );

        var session = new UserSession
        {
            CommandName = CommandName,
            MessageId = sentMessage.Id,
        };
        _userSessionService.SetUserState(user.Id, session);
    }

    public async Task HandleInputAsync(ITelegramBotClient botClient, UserSession session, Message message, BotUser user,
        string text,
        CancellationToken ct)
    {
        _logger.LogInformation("[HistoryCommand] HandleInputAsync");
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

            await botClient.SendMessage(
                message.Chat.Id,
                "Чем еще могу помочь?",
                replyMarkup: BotNavigation.Keyboards.Main,
                cancellationToken: ct
            );
            return;
        }

        if (!session.Metadata.ContainsKey("step") && text.StartsWith("report_type:"))
        {
            var typeStr = text.Split(':')[1];
            if (Enum.TryParse<ReportCategory>(typeStr, true, out var type))
            {
                session.Metadata["type"] = type.ToString();
                session.Metadata["step"] = ReportStep.Period.ToString();
                _userSessionService.SetUserState(user.Id, session);

                await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                    $"Выбрано: *{type.GetDescription()}*\n\nВыберите период:",
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: GetPeriodKeyboard(), cancellationToken: ct);
                return;
            }
        }

        var currentStepStr = session.Metadata["step"];
        var currentStep = Enum.Parse<ReportStep>(currentStepStr);
        var eventType = Enum.Parse<ReportCategory>(session.Metadata["type"]);

        session.Metadata[currentStep.ToString()] = text;

        var nextStep = ReportFlowConfig.GetNextStep(currentStep);
        if (nextStep.HasValue)
        {
            session.Metadata["step"] = nextStep.Value.ToString();
            _userSessionService.SetUserState(user.Id, session);

            await botClient.EditMessageText(
                message.Chat.Id,
                session.MessageId,
                "📅 За какой период вывести историю?",
                replyMarkup: GetPeriodKeyboard(),
                cancellationToken: ct
            );

            return;
        }

        await FinishAsync(botClient, message, user, session, ct);
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
            throw new ArgumentException("Invalid history_command input, not enough arguments");
        }

        session.Metadata.TryGetValue(nameof(ReportStep.Type), out var stepType);
        session.Metadata.TryGetValue(nameof(ReportStep.Period), out var periodStr);

        var reportCategory = Enum.Parse<ReportCategory>(stepType ?? ReportCategory.All.ToString());
        var periodMonths = int.Parse(periodStr ?? _periods.First().Key);

        var from = DateTime.UtcNow.AddMonths(-periodMonths);
        var reportRequest = new ReportRequest(
            UserId: user.TelegramId,
            Category: reportCategory,
            DateFrom: from
        );

        await _reportQueue.EnqueueReportAsync(new ReportQueueItem(
            Request: reportRequest,
            ChatId: message.Chat.Id
            ));

        await botClient.EditMessageText(
            message.Chat.Id,
            session.MessageId,
            "✅Заявка на отчет принята.\n" +
            "Как только он будет сформирован он будет отправлен отдельным файлом",
            parseMode: ParseMode.Markdown,
            cancellationToken: ct
        );
        
        _userSessionService.ClearSession(user.Id);
        
    }

    private InlineKeyboardMarkup GetPeriodKeyboard()
    {
        var rows = _periods
            .Select(p =>
                InlineKeyboardButton.WithCallbackData(p.Key, $"report_period:{p.Value.ToString()}"))
            .Chunk(3);

        return new InlineKeyboardMarkup(rows);
    }
}