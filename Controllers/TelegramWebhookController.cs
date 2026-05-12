using ArchieHealthTracker.Bot.Handlers;
using ArchieHealthTracker.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Controllers;

[ApiController]
public class TelegramWebhookController : ControllerBase
{
    private readonly BotConfiguration _config;

    public TelegramWebhookController(
        IOptions<BotConfiguration> config
    )
    {
        _config = config.Value;
    }

    [HttpPost("bots/dogs-health-tracker")]
    public async Task<IActionResult> Post(
        [FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")]
        string? receivedToken,
        [FromBody] Update update,
        [FromServices] UpdateHandler handler,
        [FromServices] ITelegramBotClient botClient,
        CancellationToken ct
    )
    {
        if (_config.SecretToken != receivedToken)
        {
            return Forbid();
        }

        try
        {
            await handler.HandlerAsync(botClient, update, ct);
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[Webhook Error] {e.Message}");
            return Ok();
        }
    }
}