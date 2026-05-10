using System.Threading.Channels;
using ArchieHealthTracker.Bot;
using ArchieHealthTracker.Bot.Handlers;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Configuration;
using ArchieHealthTracker.Data;
using ArchieHealthTracker.Domain.Entities;
using ArchieHealthTracker.Domain.Repositories;
using ArchieHealthTracker.Repositories;
using ArchieHealthTracker.Services;
using ArchieHealthTracker.Services.Reporting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("Database");
var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseMySql(dbConnectionString, serverVersion, mysqlOptions =>
    {
        mysqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorNumbersToAdd: null
        );
    });
});

builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection("BotConfiguration"));
builder.Services.AddSingleton<ITelegramBotClient>(sp =>
{
    var options = sp.GetRequiredService<IOptions<BotConfiguration>>().Value;
    return new TelegramBotClient(options.Token);
});

builder.Services.Scan(scan => scan
    .FromAssemblyOf<ITelegramCommand>()
    .AddClasses(classes => classes.AssignableTo<ITelegramCommand>())
    .AsImplementedInterfaces()
    .WithScopedLifetime());
builder.Services.AddScoped<CommandExecutor>();
builder.Services.AddScoped<UpdateHandler>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWeightRepository, WeightRepository>();
builder.Services.AddScoped<IHygieneRepository, HygieneRepository>();
builder.Services.AddScoped<ISymptomRepository, SymptomRepository>();
builder.Services.AddScoped<IMedicalEventRepository, MedicalEventRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHealthService, HealthService>();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IUserSessionService, UserSessionService>();

builder.Services.AddSingleton<IReportQueue, ReportQueue>();
builder.Services.AddHostedService<ReportProcessor>();

builder.Services.AddKeyedScoped<IReportGenerator, TelegramReportGenerator>(ReportFormat.Telegram);
builder.Services.AddKeyedScoped<IReportGenerator, PdfReportGenerator>(ReportFormat.Pdf);

builder.Services.AddScoped<UpdateHandler>();

builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection("BotConfiguration"));
builder.Services.AddHostedService<BotService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
    Console.WriteLine("✅ Миграции успешно применены.");
}

await host.RunAsync();