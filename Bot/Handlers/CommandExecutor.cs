using ArchieHealthTracker.Bot.Helpers;
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

    public CommandExecutor(IEnumerable<ITelegramCommand> commands, ILogger<CommandExecutor> logger,
        IUserSessionService userSessionService)
    {
        _commands = commands;
        _logger = logger;
        _userSessionService = userSessionService;
    }

    public async Task ExecuteCommand(string text, ITelegramBotClient bot, Message message, BotUser user, CancellationToken ct)
    {
        var userSession = _userSessionService.GetCurrentState(user.Id);
        _logger.LogInformation("Executing command :{text}", text);
        if (userSession != null)
        {
            var activeCommand = _commands.FirstOrDefault(c => c.CommandName == userSession.CommandName);
            if (activeCommand != null)
            {
                _logger.LogInformation("[CommandExecutor] have active command");
                await activeCommand.HandleInputAsync(bot, userSession, message, user, text, ct);
                return;
            }
        }
        
        var commandName = BotNavigation.Mapper.GetCommand(text) ?? text;
        var commandKey = commandName.Split(':')[0]; 

        var command = _commands.FirstOrDefault(c => 
            commandKey.Equals(c.CommandName, StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith(c.CommandName, StringComparison.OrdinalIgnoreCase));

        if (command == null)
        {
            _logger.LogWarning("Command {CommandName} not found", commandKey);
            return;
        }
        
        await command.ExecuteAsync(bot, message, user, ct);
    }
}