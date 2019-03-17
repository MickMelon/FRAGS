using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Frags.Discord.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        private readonly static Dictionary<ulong, IServiceScope> ServiceScopes = new Dictionary<ulong, IServiceScope>();

        public CommandHandler(IServiceProvider services, CommandService commands, DiscordSocketClient client)
        {
            _commands = commands;
            _services = services;
            _client = client;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(), 
                services: _services);
            _client.MessageReceived += HandleCommandAsync;
            _commands.CommandExecuted += OnCommandExecuted;
        }

        private Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            ServiceScopes[context.Message.Id].Dispose();
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) || 
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);
            var scope = _services.CreateScope();
            ServiceScopes.Add(context.Message.Id, scope);

            await _commands.ExecuteAsync(
                context: context, 
                argPos: argPos, 
                services: scope.ServiceProvider);
        }
    }
}