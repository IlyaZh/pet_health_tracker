using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Repositories;
using QuestPDF.Drawing;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace ArchieHealthTracker.Services.Reporting;

public class PdfReportGenerator : IReportGenerator
{
    static IContainer SectionHeader(IContainer container) => container
        .PaddingTop(10)
        .PaddingBottom(5)
        .BorderBottom(1.5f)
        .BorderColor(Colors.Blue.Medium)
        .DefaultTextStyle(x => x.FontSize(14).SemiBold().FontColor(Colors.Blue.Medium));

    static IContainer RowContainer(IContainer container) => container
        .PaddingVertical(4)
        .BorderBottom(1)
        .BorderColor(Colors.Grey.Lighten3);

    static IContainer ValueStyle(IContainer container) => container
        .PaddingVertical(5)
        .PaddingHorizontal(5)
        .AlignLeft();
    
    static PdfReportGenerator()
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "fonts");
        FontManager.RegisterFont(File.OpenRead(Path.Combine(fontPath, "OpenSans-Regular.ttf")));
        FontManager.RegisterFont(File.OpenRead(Path.Combine(fontPath, "OpenSans-Bold.ttf")));
    }

    public async Task<ReportResult> GenerateAsync(ReportContext context, CancellationToken ct)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);

                page.DefaultTextStyle(x => x.FontSize(12).FontFamily("Open Sans"));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("Медицинская карта Арчи").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Дата отчета: {DateTime.Now:dd.MM.yyyy}");
                    });

                    var logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Images",
                        "paw-logo.png");
                    if (File.Exists(logoPath))
                    {
                        row.ConstantItem(50).AlignMiddle().Image(logoPath);
                    }
                    else
                    {
                        row.ConstantItem(50).Placeholder();
                    }
                });

                page.Content().PaddingVertical(10).Column(column =>
                {
                    column.Spacing(10);

                    // Вес
                    if (context.WeightEntries?.Any() == true)
                {
                    column.Item().Element(SectionHeader).Text("⚖️ Антропометрия (Вес)");
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(CellStyle).Text("Дата");
                            header.Cell().Element(CellStyle).Text("Вес (кг)");
                            header.Cell().Element(CellStyle).Text("Динамика");

                            static IContainer CellStyle(IContainer container) => container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1);
                        });

                        var weights = context.WeightEntries.OrderBy(w => w.Date).ToList();
                        for (int i = 0; i < weights.Count; i++)
                        {
                            var current = weights[i];
                            var bgColor = i % 2 == 0 ? Colors.Grey.Lighten4 : Colors.White;

                            table.Cell().Background(bgColor).Element(ValueStyle).Text($"{current.Date:dd.MM.yyyy}");
                            table.Cell().Background(bgColor).Element(ValueStyle).Text($"{current.Weight.Value:F2}");

                            // Считаем дельту
                            string delta = "-";
                            if (i > 0)
                            {
                                var diff = current.Weight.Value - weights[i - 1].Weight.Value;
                                delta = diff > 0 ? $"+{diff:F2}" : $"{diff:F2}";
                            }
                            table.Cell().Background(bgColor).Element(ValueStyle).Text(delta).FontColor(delta.StartsWith('+') ? Colors.Red.Medium : Colors.Green.Medium);
                        }
                    });
                }

                    // Медицина
                    if (context.MedicalEventsEntries?.Any() == true)
                    {
                        column.Item().Text("💊 Медицинские события").FontSize(14).SemiBold();
                        foreach (var ev in context.MedicalEventsEntries)
                        {
                            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Row(row =>
                            {
                                row.ConstantItem(70).Text($"{ev.Date:dd.MM.yy}");
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text(ev.Title).SemiBold();
                                    if (!string.IsNullOrEmpty(ev.Dosage))
                                        c.Item().Text($"Дозировка: {ev.Dosage}").FontSize(9).Italic();
                                });
                            });
                        }
                    }

                    // Симптомы
                    if (context.SymptomEntries?.Any() == true)
                    {
                        column.Item().Text("🤒 Симптомы").FontSize(14).SemiBold();
                        foreach (var ev in context.SymptomEntries)
                        {
                            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Row(row =>
                            {
                                row.ConstantItem(70).Text($"{ev.CreatedAt:dd.MM.yy}");
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text(ev.Symptom.ToString()).SemiBold();
                                    if (!string.IsNullOrEmpty(ev.Note))
                                        c.Item().Text($"Заметка:\n {ev.Note}").FontSize(9)
                                            .FontColor(Colors.Grey.Darken1);
                                });
                            });
                        }
                    }

                    // Гигиена
                    if (context.HygieneEntries?.Any() == true)
                    {
                        column.Item().Text("🧼 Гигиена").FontSize(14).SemiBold();
                        foreach (var ev in context.HygieneEntries)
                        {
                            column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Padding(5).Row(row =>
                            {
                                row.ConstantItem(70).Text($"{ev.Date:dd.MM.yy}");
                                row.RelativeItem().Column(c => { c.Item().Text(ev.Event.ToString()).SemiBold(); });
                            });
                        }
                    }
                });

                page.Footer().Column(col =>
                {
                    col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
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

        var pdfBytes = document.GeneratePdf();
        return new ReportResult(
            pdfBytes,
            $"Archie_Report_{DateTime.Now:yyyyMMdd}.pdf",
            ReportFormat.Pdf
        );
    }
}
