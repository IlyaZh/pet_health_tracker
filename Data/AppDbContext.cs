using ArchieHealthTracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<BotUser> Users { get; set; }
    public DbSet<HealthEvent> Events { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<BotUser>()
            .HasIndex(u => u.TelegramId)
            .IsUnique();
        
        modelBuilder.Entity<BotUser>()
            .HasMany(u => u.Events)
            .WithOne(e => e.BotUser)
            .HasForeignKey(e => e.BotUserId);
    }

}