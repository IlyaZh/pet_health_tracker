using System.Text;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Extensions;
using ArchieHealthTracker.Domain.Interfaces.Services;

namespace ArchieHealthTracker.Application.Services.Reporting;

public class TelegramReportGenerator : IReportGenerator
{
    private const string DateFormat = "dd.MM.yy";

    public ReportResult Generate(ReportContext context, CancellationToken ct)
    {
        var reportBody = new StringBuilder();

        AppendSection(reportBody, "💊 *Медицина*", context.MedicalEventsEntries,
            ev =>
                $"• {ev.Date.ToString(DateFormat)}: *{Escape(ev.Title)}*" +
                $"{(string.IsNullOrEmpty(ev.Dosage) ? "" : $"\n  └ 💊 {Escape(ev.Dosage)}")}" +
                $"{(string.IsNullOrEmpty(ev.Note) ? "" : $"\n  └ 🗒 {Escape(ev.Note)}")}");

        AppendSection(reportBody, "⚖️ *Вес*", context.WeightEntries,
            w => $"• {w.Date.ToString(DateFormat)}: *{w.Weight.Value} кг*");

        AppendSection(reportBody, "🧼 *Гигиена*", context.HygieneEntries,
            h => $"• {h.Date.ToString(DateFormat)}: *{h.Event.GetDescription()}*");

        AppendSection(reportBody, "🤒 *Симптомы*", context.SymptomEntries,
            s =>
                $"• {s.CreatedAt.ToString(DateFormat)}: *{s.Symptom.GetDescription()}*{(string.IsNullOrEmpty(s.Note) ? "" : $"\n  └ 🗒 {Escape(s.Note)}")}");

        // Handling the empty report
        if (reportBody.Length == 0)
        {
            return new ReportResult(
                Encoding.UTF8.GetBytes("🤷‍♂️ За указанный период записей не найдено."),
                string.Empty,
                ReportFormat.Telegram
            );
        }

        // Compose the final message

        var header = new StringBuilder();
        header.AppendLine("📋 *Отчет по здоровью Арчи*");
        if (context.From.HasValue)
        {
            header.AppendLine($"🗓 Период: {context.From:dd.MM.yy} — {context.To:dd.MM.yy}");
        }

        header.AppendLine("────────────────────");
        header.Append(reportBody);

        return new ReportResult(
            Encoding.UTF8.GetBytes(header.ToString()),
            string.Empty,
            ReportFormat.Telegram
        );
    }

    private void AppendSection<T>(StringBuilder sb, string title, IEnumerable<T>? items, Func<T, string> formatter)
    {
        if (items == null || !items.Any()) return;

        sb.AppendLine($"\n{title}");
        foreach (var item in items)
        {
            sb.AppendLine(formatter(item));
        }
    }

    private static string Escape(string text)
    {
        return text.Replace("_", "\\_")
            .Replace("*", "\\*")
            .Replace("[", "\\[")
            .Replace("`", "\\`")
            .Replace("#", "");
    }
}