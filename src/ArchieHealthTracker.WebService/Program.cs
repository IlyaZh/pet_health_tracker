using ArchieHealthTracker.Application;
using ArchieHealthTracker.Bot;
using ArchieHealthTracker.Infrastructure;
using ArchieHealthTracker.Infrastructure.Data;
using ArchieHealthTracker.WebService.Extensions;
using Microsoft.EntityFrameworkCore;
using Serilog;

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilogLogging(builder.Configuration);

    var dbConnectionString = builder.Configuration.GetConnectionString("Database");

    builder.Services.AddControllers();

    // Register Layers
    builder.Services.AddInfrastructure(dbConnectionString!);
    builder.Services.AddApplication();
    builder.Services.AddBotServices(builder.Configuration);

    builder.Services.AddHealthChecks();

    var app = builder.Build();

    app.MapHealthChecks("/health");
    app.MapControllers();

    // Auto-migrate on startup
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("✅ Миграции успешно применены.");
    }

    Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
