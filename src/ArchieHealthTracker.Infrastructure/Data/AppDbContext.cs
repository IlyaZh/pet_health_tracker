using ArchieHealthTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ArchieHealthTracker.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<BotUser> Users { get; set; }
    public DbSet<HealthEvent> Events { get; set; }
    public DbSet<WeightEntry> Weights { get; set; }
    public DbSet<HygieneEntry> Hygiene { get; set; }
    public DbSet<SymptomEntry> Symptoms { get; set; }
    public DbSet<MedicalEventEntry> MedicalEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasCharSet("utf8mb4")
            .UseCollation("utf8mb4_unicode_ci");

        modelBuilder.Entity<HealthEvent>(entity => { entity.ToTable("health_events"); });

        modelBuilder.Entity<BotUser>(entity =>
        {
            entity.ToTable("users");
            entity.HasIndex(u => u.TelegramId)
                .IsUnique();
        });

        modelBuilder.Entity<WeightEntry>(entity =>
        {
            entity.ToTable("weight");

            entity.HasIndex(e => e.Date).IsUnique();

            entity.Property(e => e.Weight)
                .HasConversion(
                    v => v.Value,
                    v => Weight.FromKilograms(v))
                .HasPrecision(5, 2)
                .HasColumnType("decimal(5,2)");
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HygieneEntry>(entity =>
        {
            entity.ToTable("hygiene");
            entity.HasIndex(e => e.Date);
            entity.HasIndex(e => e.Event);

            entity.Property(e => e.Event)
                .HasConversion<string>();
            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.Date, e.Event }).IsUnique();
        });

        modelBuilder.Entity<SymptomEntry>(symptom =>
        {
            symptom.ToTable("symptoms");
            symptom.HasIndex(e => e.CreatedAt);
            symptom.HasIndex(e => new { e.Symptom, e.CreatedAt });

            symptom.Property(s => s.Symptom)
                .HasConversion<string>();
            symptom.HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MedicalEventEntry>(medicalEvent =>
        {
            medicalEvent.ToTable("medical_events");
            medicalEvent.HasIndex(e => e.Date);
            medicalEvent.Property(e => e.Type)
                .HasConversion<string>();
            medicalEvent.HasIndex(e => new { e.Type, e.Date });
            medicalEvent.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}