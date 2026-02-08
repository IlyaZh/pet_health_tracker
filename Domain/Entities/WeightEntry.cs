namespace ArchieHealthTracker.Entities;

public class WeightEntry
{
        public Guid Id { get; set; }
        
        public Weight Weight { get; set; }
        
        public DateOnly Date { get; set; }

        public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
        
        public Guid UserId { get; set; }
        
        public BotUser BotUser { get; set; } = null!;
}