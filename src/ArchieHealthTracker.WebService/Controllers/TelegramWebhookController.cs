using ArchieHealthTracker.Bot.Handlers;
using ArchieHealthTracker.WebService.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.WebService.Controllers;

[ApiController]
public class TelegramWebhookController(IOptions<ServiceConfiguration> config) : ControllerBase
{
    private readonly ServiceConfiguration _config = config.Value;

    [HttpPost("bot/webhook")]
    public async Task<IActionResult> Post(
        [FromHeader(Name = "X-Telegram-Bot-Api-Secret-Token")]
        string? receivedToken,
        [FromBody] Update update,
        [FromServices] UpdateHandler handler,
        [FromServices] ITelegramBotClient botClient,
        CancellationToken ct
    )
    {
        if (_config.SecretToken != receivedToken) return Forbid();

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