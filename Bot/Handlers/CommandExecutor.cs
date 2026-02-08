using ArchieHealthTracker.Bot.Interfaces;
using ArchieHealthTracker.Entities;
using ArchieHealthTracker.Services;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ArchieHealthTracker.Bot.Handlers;

public class CommandExecutor
{
    private readonly IEnumerable<ITelegramCommand> _commands;
    private readonly ILogger<CommandExecutor> _logger;
    private readonly IUserSessionService _userSessionService;

    public CommandExecutor(IEnumerable<ITelegramCommand> commands, ILogger<CommandExecutor> logger,  IUserSessionService userSessionService)
    {
        _commands = commands;
        _logger = logger;
        _userSessionService =  userSessionService;
    }

    public async Task ExecuteCommand(string commandName, ITelegramBotClient bot, Message message, BotUser user, CancellationToken cancellationToken) 
    {
        var activeCommandName = _userSessionService.GetCurrentCommand(user.Id);

        if (!string.IsNullOrEmpty(activeCommandName))
        {
            var activeCommand = _commands.FirstOrDefault(c => c.CommandName == activeCommandName);
            if (activeCommand != null)
            {
                await activeCommand.HandleInputAsync(bot, message, user, cancellationToken);
                return;
            }
        }

        var command =  _commands.FirstOrDefault(c => commandName.Equals(c.CommandName, StringComparison.OrdinalIgnoreCase));
        if (command == null)
        {
            await bot.SendMessage(message.Chat.Id, "Я не знаю такую команду 🤔", cancellationToken: cancellationToken);
            _logger.LogWarning("Unknown command: {CommandName}", commandName);
            return;
        }
        
        await command.ExecuteAsync(bot, message, user, cancellationToken);
    }
}