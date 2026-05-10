using System.Text;
using ArchieHealthTracker.Domain.Entities; 
using ArchieHealthTracker.Extensions;
using ArchieHealthTracker.Domain.Repositories;

namespace ArchieHealthTracker.Services.Reporting;

public class TelegramReportGenerator : IReportGenerator
{
    public Task<ReportResult> GenerateAsync(ReportContext context, CancellationToken ct)
    {
        var sb = new StringBuilder();

        if (context.MedicalEventsEntries is { Count: > 0 })
        {
            sb.AppendLine("\n💊 *Медицина*");
            foreach (var ev in context.MedicalEventsEntries)
            {
                sb.AppendLine($"• {ev.Date:dd.MM}: *{Escape(ev.Title)}*");
                if (!string.IsNullOrEmpty(ev.Dosage))
                    sb.AppendLine($"  └ 💊 Доза: {Escape(ev.Dosage)}");
            }
        }

        if (context.WeightEntries is { Count: > 0 })
        {
            sb.AppendLine("\n⚖️ *Вес*");
            foreach (var w in context.WeightEntries)
            {
                sb.AppendLine($"• {w.Date:dd.MM}: *{w.Weight.Value} кг*");
            }
        }

        if (context.HygieneEntries is { Count: > 0 })
        {
            sb.AppendLine("\n🧼 *Гигиена*");
            foreach (var h in context.HygieneEntries)
            {
                sb.AppendLine($"• {h.Date:dd.MM}: *{h.Event.GetDescription()}*");
            }
        }

        if (context.SymptomEntries is { Count: > 0 })
        {
            sb.AppendLine("\n🤒 *Симптомы*");
            foreach (var s in context.SymptomEntries)
            {
                sb.AppendLine($"• {s.CreatedAt:dd.MM}: *{s.Symptom.GetDescription()}*");
            }
        }

        if (sb.Length == 0)
        {
            var text = "🤷‍♂️ За указанный период записей не найдено.";
            return Task.FromResult(new ReportResult(
                Encoding.UTF8.GetBytes(text),
                string.Empty,
                ReportFormat.Telegram
            ));
        }

        var header = new StringBuilder();
        header.AppendLine("📋 *Отчет по здоровью Арчи*");
        if (context.From.HasValue)
        {
            header.AppendLine($"🗓 Период: {context.From:dd.MM} — {context.To:dd.MM}");
        }

        header.AppendLine("────────────────────");
        header.Append(sb);

        return Task.FromResult(new ReportResult(
            Encoding.UTF8.GetBytes(header.ToString()),
            string.Empty,
            ReportFormat.Telegram
        ));
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