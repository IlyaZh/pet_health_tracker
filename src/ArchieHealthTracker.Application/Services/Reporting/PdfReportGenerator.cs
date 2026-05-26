using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Extensions;
using ArchieHealthTracker.Domain.Interfaces.Services;
using QuestPDF;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ArchieHealthTracker.Application.Services.Reporting;

public class PdfReportGenerator : IReportGenerator
{
    static PdfReportGenerator()
    {
        Settings.License = LicenseType.Community;

        var fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Fonts");
        FontManager.RegisterFont(File.OpenRead(Path.Combine(fontPath, "OpenSans-Regular.ttf")));
        FontManager.RegisterFont(File.OpenRead(Path.Combine(fontPath, "OpenSans-Bold.ttf")));
    }

    public ReportResult Generate(ReportContext context, CancellationToken ct)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Open Sans"));

                // 1. Header
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Медицинская карта Арчи").FontSize(22).SemiBold().FontColor(Colors.Blue.Medium);

                        var dataRange = context.From.HasValue
                            ? $"🗓 период: {context.From:dd.MM.yyyy} — {context.To:dd.MM.yyyy}"
                            : $"🗓 на дату: {DateTime.Now:dd.MM.yyyy}";

                        col.Item().Text(dataRange).FontSize(10).FontColor(Colors.Grey.Darken1);
                    });

                    var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images",
                        "paw-logo.png");
                    if (File.Exists(logoPath))
                        row.ConstantItem(50).AlignMiddle().Image(logoPath);
                    else
                        row.ConstantItem(50).Placeholder();
                });

                // 2. Content
                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(20);

                    // --- ВЕС ---
                    if (context.WeightEntries?.Any() == true)
                    {
                        var weights = context.WeightEntries.OrderBy(w => w.Date).ToList();
                        var rows = new List<string[]>();

                        for (var i = 0; i < weights.Count; i++)
                        {
                            var current = weights[i];
                            var delta = "-";
                            if (i > 0)
                            {
                                var diff = current.Weight.Value - weights[i - 1].Weight.Value;
                                delta = diff > 0 ? $"+{diff:F2}" : $"{diff:F2}";
                            }

                            rows.Add([current.Date.ToString("dd.MM.yyyy"), $"{current.Weight.Value:F2} кг", delta]);
                        }

                        DrawTable(column.Item(), "⚖️ Антропометрия", ["Дата", "Вес", "Δ"], rows, [false, false, false],
                            (row, col, val) => col == 2 && val.StartsWith('+') ? Colors.Red.Medium :
                                col == 2 && val.StartsWith('-') ? Colors.Green.Medium : Colors.Black);
                    }

                    // --- МЕДИЦИНА ---
                    if (context.MedicalEventsEntries?.Any() == true)
                    {
                        var rows = context.MedicalEventsEntries
                            .OrderByDescending(m => m.Date)
                            .Select(m => new[]
                            {
                                m.Date.ToString("dd.MM.yy"),
                                m.Title,
                                $"Доз: {m.Dosage ?? "-"}",
                                $"Заметка: {m.Note ?? "-"}"
                            }).ToList();

                        DrawTable(column.Item(), "💊 Медицинские события",
                            ["Дата", "Событие", "Дозировка", "Комментарий"], rows,
                            [false, false, false, true]);
                    }

                    // --- СИМПТОМЫ ---
                    if (context.SymptomEntries?.Any() == true)
                    {
                        var rows = context.SymptomEntries
                            .OrderByDescending(s => s.CreatedAt)
                            .Select(s => new[]
                            {
                                s.CreatedAt.ToString("dd.MM.yy"),
                                s.Symptom.GetDescription(),
                                s.Note ?? "-"
                            }).ToList();

                        DrawTable(column.Item(), "🤒 Симптомы", ["Дата", "Тип", "Заметка"], rows, [false, false, true]);
                    }

                    // --- ГИГИЕНА ---
                    if (context.HygieneEntries?.Any() == true)
                    {
                        var rows = context.HygieneEntries
                            .OrderByDescending(h => h.Date)
                            .Select(e => new[]
                            {
                                e.Date.ToString("dd.MM.yy"),
                                e.Event.GetDescription()
                            }).ToList();

                        DrawTable(column.Item(), "🧼 Гигиена", ["Дата", "Процедура"], rows, [false, true]);
                    }
                });

                // 3. Footer
                page.Footer().Column(col =>
                {
                    col.Item().LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);
                    col.Item().PaddingTop(5).Row(row =>
                    {
                        row.RelativeItem().Text("Сгенерировано Archie Health Tracker").FontSize(8)
                            .FontColor(Colors.Grey.Medium);
                        row.RelativeItem().AlignRight().Text(x =>
                        {
                            x.Span("Страница ");
                            x.CurrentPageNumber();
                        });
                    });
                });
            });
        });

        return new ReportResult(document.GeneratePdf(), $"Archie_Report_{DateTime.Now:yyyyMMdd}.pdf", ReportFormat.Pdf);
    }

    private static IContainer SectionHeader(IContainer container)
    {
        return container
            .PaddingTop(10)
            .PaddingBottom(5)
            .BorderBottom(1.5f)
            .BorderColor(Colors.Blue.Medium)
            .DefaultTextStyle(x => x.FontSize(14).SemiBold().FontColor(Colors.Blue.Medium));
    }

    private static IContainer ValueStyle(IContainer container)
    {
        return container
            .PaddingVertical(5)
            .PaddingHorizontal(5)
            .AlignLeft();
    }

    private void DrawTable(IContainer container, string title, string[] headers, List<string[]> rows, bool[] isFlexible,
        Func<int, int, string, Color>? colorPicker = null)
    {
        container.Column(col =>
        {
            col.Item().Element(SectionHeader).Text(title);
            col.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    for (var i = 0; i < headers.Length; i++)
                        if (isFlexible[i]) columns.RelativeColumn(3);
                        else columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    foreach (var h in headers)
                        header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).Text(h)
                            .SemiBold();
                });

                for (var i = 0; i < rows.Count; i++)
                {
                    var row = rows[i];
                    var bgColor = i % 2 == 0 ? Colors.Grey.Lighten5 : Colors.White;

                    for (var j = 0; j < headers.Length; j++)
                    {
                        var val = row[j];
                        table.Cell().Background(bgColor).Element(ValueStyle).Text(val)
                            .FontColor(colorPicker?.Invoke(i, j, val) ?? Colors.Black);
                    }
                }
            });
        });
    }
}