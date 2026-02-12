using ArchieHealthTracker.Bot;
using ArchieHealthTracker.Bot.Handlers;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Configuration;
using ArchieHealthTracker.Data;
using ArchieHealthTracker.Repositories;
using ArchieHealthTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("Database");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString)));

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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHealthService, HealthService>();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<UserSessionService>();

builder.Services.AddScoped<UpdateHandler>();

builder.Services.Configure<BotConfiguration>(builder.Configuration.GetSection("BotConfiguration"));
builder.Services.AddHostedService<BotService>();

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); 
}

await host.RunAsync();