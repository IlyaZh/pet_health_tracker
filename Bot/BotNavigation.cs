using Telegram.Bot.Types.ReplyMarkups;

namespace ArchieHealthTracker.Bot.Helpers;

public static class BotNavigation
{
    private const int ButtonsInRow = 2;
    private static readonly IReadOnlyDictionary<string, string> _menu = new Dictionary<string, string>
    {
        ["⚖️ Взвесить"] = "/weight",
        ["🧼 Гигиена"] = "/hygiene",
        ["🤒 Симптомы"] = "/symptom",
        ["📋 Отчет"] = "/report"
    };
    
    public static class Mapper
    {
        public static string? GetCommand(string label) => 
            _menu.TryGetValue(label, out var cmd) ? cmd : null;
    }
    public static class Keyboards
    {
        public static readonly ReplyKeyboardMarkup Main = new(
            _menu.Keys
                .Select(k => new KeyboardButton(k))
                .ChunkBy(ButtonsInRow) 
        )
        {
            ResizeKeyboard = true,
            InputFieldPlaceholder = "Выберите действие"
        };  
    }
    
    public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int size) =>
        source.Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / size)
            .Select(x => x.Select(v => v.Value).ToList());
}