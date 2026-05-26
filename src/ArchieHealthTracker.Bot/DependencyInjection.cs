using ArchieHealthTracker.Application.Interfaces;
using ArchieHealthTracker.Bot.Configuration;
using ArchieHealthTracker.Bot.Handlers;
using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Bot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace ArchieHealthTracker.Bot;

public static class DependencyInjection
{
    public static IServiceCollection AddBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BotConfiguration>(configuration.GetSection("BotConfiguration"));

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BotConfiguration>>().Value;
            return new TelegramBotClient(options.Token);
        });

        services.Scan(scan => scan
            .FromAssemblyOf<ITelegramCommand>()
            .AddClasses(classes => classes.AssignableTo<ITelegramCommand>())
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.AddSingleton<INotificationService, TelegramNotificationService>();
        services.AddScoped<CommandExecutor>();
        services.AddScoped<UpdateHandler>();
        services.AddHostedService<BotService>();

        return services;
    }
}
