using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Frags.Presentation.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Frags.Discord.Services
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CharacterController _charController;
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        private readonly static Dictionary<ulong, IServiceScope> _serviceScopes = new Dictionary<ulong, IServiceScope>();

        public CommandHandler(IServiceProvider services, CharacterController charController, CommandService commands, DiscordSocketClient client)
        {
            _commands = commands;
            _charController = charController;
            _services = services;
            _client = client;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(), 
                services: _services);
            _client.MessageReceived += async (msg) => _ = Task.Run(() => HandleCommandAsync(msg));
            _commands.CommandExecuted += OnCommandExecuted;
        }

        private Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            _serviceScopes[context.Message.Id].Dispose();
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            var message = msg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix('!', ref argPos) || 
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
            {
                if (!message.Author.IsBot)
                    await _charController.GiveExperienceAsync(message.Author.Id, message.Channel.Id, message.Content);
                    
                return;
            }

            var context = new SocketCommandContext(_client, message);
            var scope = _services.CreateScope();
            _serviceScopes.Add(context.Message.Id, scope);

            await _commands.ExecuteAsync(
                context: context, 
                argPos: argPos, 
                services: scope.ServiceProvider);
        }
    }
}