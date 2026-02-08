namespace ArchieHealthTracker.Entities;

public enum HealthEventType
{
    General = 0,
    Weight = 1,        // Вес
    Symptom = 2,       // Рвота, диарея, вялость
    Medicine = 3,      // Таблетки, обработки
    Vaccination = 4,   // Прививки
    Hygiene = 5,       // Когти, зубы
    Food = 6           // Смена корма / реакция
}

public enum HygieneEventType
{
    Unknown = 0,    // Защита от дефолтных значений
    Nails = 1,      // Стрижка когтей
    Glands = 2,     // Чистка желез
    Teeth = 3      // Чистка зубов
}