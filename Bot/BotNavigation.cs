using Telegram.Bot.Types.ReplyMarkups;

namespace ArchieHealthTracker.Bot.Helpers;

public static class BotNavigation
{
    private const int ButtonsInRow = 2;

    private static readonly IReadOnlyDictionary<string, string> Menu = new Dictionary<string, string>
    {
        ["⚖️ Вес"] = "/weight",
        ["🧼 Гигиена"] = "/hygiene",
        ["🤒 Симптомы"] = "/symptom",
        ["💊 Медицина"] = "/medical_event",
        ["📋 Отчет"] = "/history",
    };

    private static readonly IReadOnlyDictionary<string, string> MenuAliases = new Dictionary<string, string>
    {
        ["/menu"] = "/start",
        ["/cancel"] = "/cancel"
    };

    public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> source, int size) =>
        source.Select((x, i) => new { Index = i, Value = x })
            .GroupBy(x => x.Index / size)
            .Select(x => x.Select(v => v.Value).ToList());

    public static class Mapper
    {
        public static string? GetCommand(string label)
        {
            return MenuAliases.GetValueOrDefault(label) ?? Menu.GetValueOrDefault(label);
        }
    }

    public static class Keyboards
    {
        public static readonly ReplyKeyboardMarkup Main = new(
            Menu.Keys
                .Select(k => new KeyboardButton(k))
                .Chunk(ButtonsInRow)
        )
        {
            ResizeKeyboard = true,
            InputFieldPlaceholder = "Выберите действие"
        };
    }
}