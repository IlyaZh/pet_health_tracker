using ArchieHealthTracker.Domain.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Interfaces;

/// <summary>
/// Defines a command that can be executed by the Telegram bot.
/// </summary>
public interface ITelegramCommand
{
    /// <summary>
    /// The unique name of the command (e.g., "/start", "Weight").
    /// </summary>
    string CommandName { get; }

    /// <summary>
    /// Executes the initial command logic when the command is triggered.
    /// </summary>
    /// <param name="botClient">The Telegram bot client instance.</param>
    /// <param name="message">The message that triggered the command.</param>
    /// <param name="user">The user who sent the message.</param>
    /// <param name="ct">Cancellation token.</param>
    Task ExecuteAsync(
        ITelegramBotClient botClient,
        Message message,
        BotUser user,
        CancellationToken ct
    );

    /// <summary>
    /// Handles user input when the command is in a conversational flow.
    /// </summary>
    /// <param name="botClient">The Telegram bot client instance.</param>
    /// <param name="session">The current user session and state.</param>
    /// <param name="message">The incoming message from the user.</param>
    /// <param name="user">The user who sent the message.</param>
    /// <param name="text">The text content of the message.</param>
    /// <param name="ct">Cancellation token.</param>
    Task HandleInputAsync(
        ITelegramBotClient botClient,
        UserSession session,
        Message message,
        BotUser user,
        string text,
        CancellationToken ct
    );
}
