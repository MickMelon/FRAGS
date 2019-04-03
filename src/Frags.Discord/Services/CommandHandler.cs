using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly GeneralOptions _options;

        private readonly static Dictionary<ulong, IServiceScope> _serviceScopes = new Dictionary<ulong, IServiceScope>();

        public CommandHandler(IServiceProvider services,
            CharacterController charController,
            CommandService commands,
            DiscordSocketClient client,
            GeneralOptions options)
        {
            _commands = commands;
            _charController = charController;
            _services = services;
            _client = client;
            _options = options;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModulesAsync(
                assembly: Assembly.GetEntryAssembly(), 
                services: _services);

            #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            _client.MessageReceived += async (msg) => _ = Task.Run(() => HandleCommandAsync(msg));
            #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            _commands.CommandExecuted += OnCommandExecuted;
        }

        private Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            _serviceScopes[context.Message.Id].Dispose();
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage message)) return;

            int argPos = 0;

            if (!(message.HasCharPrefix(_options.CommandPrefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos)))
            {
                if (!message.Author.IsBot)
                {
                    bool leveledUp = await _charController.GiveExperienceAsync(message.Author.Id, message.Channel.Id, message.Content);

                    if (leveledUp)
                    {
                        await msg.Author.SendMessageAsync("Level up!");
                    }
                }

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