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
        _logger.LogInformation("[CommandExecutor] Executing command: {text}", text);
        
        if (!string.IsNullOrEmpty(text) && text.StartsWith("/"))
        {
            _logger.LogInformation("[CommandExecutor] Global command detected. Clearing session for user {UserId}", user.Id);
            _userSessionService.ClearSession(user.Id); 
            
            await FindAndExecuteNewCommand(text, bot, message, user, ct);
            return;
        }
        
        var userSession = _userSessionService.GetCurrentState(user.Id);
        if (userSession != null)
        {
            var activeCommand = _commands.FirstOrDefault(c => c.CommandName == userSession.CommandName);
            if (activeCommand != null)
            {
                _logger.LogInformation("[CommandExecutor] Routing to active command: {Command}", activeCommand.CommandName);
                await activeCommand.HandleInputAsync(bot, userSession, message, user, text, ct);
                return;
            }
            
            _userSessionService.ClearSession(user.Id);
        }
        
        await FindAndExecuteNewCommand(text, bot, message, user, ct);
    }
    
    private async Task FindAndExecuteNewCommand(string text, ITelegramBotClient bot, Message message, BotUser user, CancellationToken ct)
    {
        var commandName = BotNavigation.Mapper.GetCommand(text) ?? text;
        var commandKey = commandName.Split(':')[0];

        var command = _commands.FirstOrDefault(c =>
            commandKey.Equals(c.CommandName, StringComparison.OrdinalIgnoreCase) ||
            text.StartsWith(c.CommandName, StringComparison.OrdinalIgnoreCase));

        if (command == null)
        {
            _logger.LogWarning("[CommandExecutor] Command {CommandName} not found", commandKey);
            return;
        }

        await command.ExecuteAsync(bot, message, user, ct);
    }
}