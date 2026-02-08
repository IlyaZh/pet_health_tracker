using ArchieHealthTracker.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<BotUser> Users { get; set; }
    public DbSet<HealthEvent> Events { get; set; }
    public DbSet<WeightEntry> Weights { get; set; }

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

        modelBuilder.Entity<WeightEntry>(entity =>
        {
            entity.ToTable("weight");

            entity.HasIndex(e => e.Date).IsUnique();
            
            entity.Property(e => e.Weight)
                .HasConversion(
                    v => v.Value,
                    v => Weight.FromKilograms(v))
                .HasPrecision(5, 2)
                .HasColumnType("decimal(5,2)") 
                .HasColumnName("weight_kg");
            entity.HasOne(e => e.BotUser)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

}