using ArchieHealthTracker.Bot.Configuration;
using ArchieHealthTracker.Bot.Handlers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.WebService.Controllers;

/// <summary>
/// Webhook controller for receiving updates from the Telegram Bot API.
/// </summary>
[ApiController]
public class TelegramWebhookController(IOptions<BotConfiguration> config) : ControllerBase
{
    private readonly BotConfiguration _config = config.Value;

    /// <summary>
    /// Processes an incoming update from Telegram.
    /// </summary>
    /// <param name="receivedToken">The secret token provided in the X-Telegram-Bot-Api-Secret-Token header.</param>
    /// <param name="update">The update object containing information about user interaction.</param>
    /// <param name="handler">The handler responsible for processing different types of updates.</param>
    /// <param name="botClient">The bot client instance.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the processing.</returns>
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
        if (_config.SecretToken != receivedToken)
            return StatusCode(StatusCodes.Status403Forbidden);

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
