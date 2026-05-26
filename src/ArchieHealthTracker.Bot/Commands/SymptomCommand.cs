using ArchieHealthTracker.Application.Services;
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

public class SymptomCommand(
    IUserSessionService userSessionService,
    IHealthService healthService,
    ILogger<SymptomCommand> logger)
    : ITelegramCommand
{
    private readonly string _whatHappened = "Что случилось с Арчи?";
    public string CommandName { get; } = "/symptom";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message, BotUser user,
        CancellationToken ct)
    {
        var buttons = Enum.GetValues<SymptomType>()
            .Where(type => type != SymptomType.Unknown)
            .Select(type => InlineKeyboardButton.WithCallbackData(type.GetDescription(), $"symptom:{type.ToString()}"))
            .Chunk(3);

        var sentMessage = await botClient.SendMessage(
            message.Chat.Id,
            _whatHappened,
            replyMarkup: new InlineKeyboardMarkup(buttons),
            cancellationToken: ct
        );

        userSessionService.SetUserState(user.Id, new UserSession
        {
            CommandName = CommandName,
            MessageId = sentMessage.Id
        });
    }

    public async Task HandleInputAsync(
        ITelegramBotClient botClient,
        UserSession session,
        Message message,
        BotUser user,
        string text,
        CancellationToken ct
    )
    {
        logger.LogInformation($"[SymptomCommand] Received text/callback: {text}");

        if (!string.IsNullOrEmpty(text) && text.StartsWith("symptom:"))
        {
            var parts = text.Split(':');

            if (parts[1] == "skip")
            {
                await FinalizeSymptomAsync(botClient, session, message, user, null, ct);
                return;
            }

            if (Enum.TryParse<SymptomType>(parts[1], out var type))
            {
                session.Metadata["type"] = parts[1]; // Сохраняем имя (Limping)
                session.Metadata["step"] = "awaiting_details";
                userSessionService.SetUserState(user.Id, session);

                await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                    $"Выбран симптом: *{type.GetDescription()}*\n\nНапиши подробности (например, 'после прогулки') или нажми кнопку 'Пропустить':",
                    ParseMode.Markdown,
                    new InlineKeyboardMarkup(
                        InlineKeyboardButton.WithCallbackData("⏩ Пропустить", "symptom:skip")),
                    cancellationToken: ct);
                return;
            }
        }

        if (session.Metadata.TryGetValue("type", out var storedTypeName))
            await FinalizeSymptomAsync(botClient, session, message, user, text, ct);
    }

    private async Task FinalizeSymptomAsync(ITelegramBotClient botClient, UserSession session, Message message,
        BotUser user, string? note, CancellationToken ct)
    {
        var typeName = session.Metadata["type"];
        if (Enum.TryParse<SymptomType>(typeName, out var type))
        {
            var symptom = new Symptom
            {
                Type = type,
                Note = note
            };

            await healthService.AddSymptomAsync(user, symptom, ct);

            await botClient.EditMessageText(message.Chat.Id, session.MessageId,
                $"✅ Записал симптом: *{type.GetDescription()}*\nДетали: _{note ?? "нет"}_",
                ParseMode.Markdown,
                cancellationToken: ct);

            userSessionService.ClearSession(user.Id);
        }
    }
}