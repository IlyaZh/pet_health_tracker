using ArchieHealthTracker.Bot;
using ArchieHealthTracker.Bot.Handlers;
using ArchieHealthTracker.Data;
using ArchieHealthTracker.Repositories;
using ArchieHealthTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("Database");
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString)));

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<UpdateHandler>();

builder.Services.AddHostedService<BotService>();

var host = builder.Build();
await host.RunAsync();