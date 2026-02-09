using Telegram.Bot.Types.ReplyMarkups;

namespace ArchieHealthTracker.Bot.Keyboards;

public static class MainMenuKeyboard
{
    public static ReplyKeyboardMarkup Get()
    {
        return new ReplyKeyboardMarkup(new[]
        {
            new[] { new KeyboardButton("⚖️ Взвесить"), new KeyboardButton("🧼 Гигиена") },
            new[] { new KeyboardButton("🤒 Симптомы"), new KeyboardButton("📋 Отчет") }
        })
        {
            ResizeKeyboard = true // Чтобы кнопки не были огромными
        };
    }
}