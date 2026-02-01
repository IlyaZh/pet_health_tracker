using ArchieHealthTracker.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<BotService>();

var host = builder.Build();
await host.RunAsync();