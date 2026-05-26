using System.Text;
using ArchieHealthTracker.Application.Interfaces;
using ArchieHealthTracker.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ArchieHealthTracker.Bot.Services;

public class TelegramNotificationService(
    ITelegramBotClient botClient) : INotificationService
{
    public async Task SendAsync(long chatId, ReportResult reportResult, CancellationToken ct)
    {
        if (reportResult.Format == ReportFormat.Telegram)
        {
            await botClient.SendMessage(
                chatId,
                Encoding.UTF8.GetString((byte[])reportResult.Content),
                ParseMode.Markdown,
                cancellationToken: ct
            );
        }
        else
        {
            using var ms = new MemoryStream(reportResult.Content);
            await botClient.SendDocument(
                chatId,
                InputFile.FromStream(ms, reportResult.FileName),
                "Твой отчет готов! 🐾",
                cancellationToken: ct
            );
        }
    }
}
