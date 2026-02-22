using System.Text;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Extensions;
using ArchieHealthTracker.Repositories;

namespace ArchieHealthTracker.Services.Reporting;

public class TelegramReportGenerator : IReportGenerator<string>
{
    public Task<string> GenerateAsync(ReportContext context, CancellationToken ct)
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
            return Task.FromResult("🤷‍♂️ За указанный период записей не найдено.");
        }

        var header = new StringBuilder();
        header.AppendLine("📋 *Отчет по здоровью Арчи*");
        // Можно добавить: header.AppendLine($"🗓 Период: {context.From:dd.MM} — {context.To:dd.MM}");
        header.AppendLine("────────────────────");
        header.Append(sb);

        return Task.FromResult(header.ToString());
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