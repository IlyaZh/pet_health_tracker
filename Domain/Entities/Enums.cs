using System.ComponentModel;

namespace ArchieHealthTracker.Entities;

public enum HealthEventType
{
    General = 0,
    Weight = 1, // Вес
    Symptom = 2, // Рвота, диарея, вялость
    Medicine = 3, // Таблетки, обработки
    Vaccination = 4, // Прививки
    Hygiene = 5, // Когти, зубы
    Food = 6 // Смена корма / реакция
}

public enum HygieneEventType
{
    [Description("Неизвестно")]
    Unknown = 0,

    [Description("Стрижка когтей ✂️")]
    Nails = 1,

    [Description("Чистка желез 🍑")]
    Glands = 2,

    [Description("Чистка зубов 🦷")]
    Teeth = 3
}

public enum SymptomType
{
    [Description("Неизвестно")]
    Unknown = 0,

    [Description("Рвота 🤮")]
    Vomiting = 1,

    [Description("Диарея 💩")]
    Diarrhea = 2,

    [Description("Вялость 🥱")]
    Lethargy = 3,

    [Description("Отказ от еды 🥣🚫")]
    LossOfAppetite = 4,

    [Description("Хромота 🐾🩹")]
    Limping = 5,

    [Description("Зуд / Чес 🧼🐕")]
    Itching = 6,

    [Description("Другое ❓")]
    Other = 99
}