using ArchieHealthTracker.Bot.Handlers;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Controllers;

[ApiController]
public class TelegramWebhookController : ControllerBase
{
    [HttpPost("bots/dogs-health-tracker")]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        [FromServices] UpdateHandler handler,
        [FromServices] ITelegramBotClient botClient,
        CancellationToken ct
    )
    {
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