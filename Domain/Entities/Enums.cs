using System.ComponentModel;

namespace ArchieHealthTracker.Entities;

public enum ReportCategory
{
    [Description("Неизвестно")]
    Unknown = 0,
    
    [Description("Все")]
    All = 1,
    
    [Description("🧼 Гигиена")]
    Hygiene = 2,
    
    [Description("💊 Медицина")]
    MedicalEvent = 3,
    
    [Description("🤒 Симптомы")]
    Symptom = 4,
    
    [Description("⚖ Вес")]
    Weight = 5,
};

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

public enum MedicalEventType
{
    Unknown = 0,
    [Description("Вакцинация 💉")] Vaccination = 1,
    [Description("От клещей/глистов 🛡️")] ParasiteTreatment = 2,
    [Description("Прием лекарств 💊")] Medication = 3,
    [Description("Осмотр врача 🩺")] VetVisit = 4
}
