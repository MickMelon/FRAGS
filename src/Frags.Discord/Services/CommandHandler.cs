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
        private readonly CommandService _commands;
        private readonly IServiceProvider _services;

        private readonly static Dictionary<ulong, IServiceScope> _serviceScopes = new Dictionary<ulong, IServiceScope>();

        public CommandHandler(IServiceProvider services,
            CommandService commands,
            DiscordSocketClient client)
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
            #pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            _client.MessageReceived += async (msg) => _ = Task.Run(() => GiveExperienceAsync(msg));
            #pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            _commands.CommandExecuted += OnCommandExecuted;
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            _serviceScopes[context.Message.Id].Dispose();
            _serviceScopes.Remove(context.Message.Id);

            try { if (!context.Message.Author.IsBot) await context.Message.DeleteAsync(); } catch { }
        }

        private async Task HandleCommandAsync(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage message)) return;

            int argPos = 0;

            var scope = _services.CreateScope();
            var options = scope.ServiceProvider.GetRequiredService<GeneralOptions>();

            if (!(message.HasCharPrefix(options.CommandPrefix, ref argPos) ||
                message.HasMentionPrefix(_client.CurrentUser, ref argPos))
                && !message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);
            _serviceScopes.Add(context.Message.Id, scope);

            await _commands.ExecuteAsync(
                context: context, 
                argPos: argPos, 
                services: scope.ServiceProvider);
        }

        private async Task GiveExperienceAsync(SocketMessage msg)
        {
            if (!(msg is SocketUserMessage message)) return;
            int _ = 0;

            using (var scope = _services.CreateScope())
            {
                var options = scope.ServiceProvider.GetRequiredService<GeneralOptions>();

                if (!(message.HasCharPrefix(options.CommandPrefix, ref _) ||
                    message.HasMentionPrefix(_client.CurrentUser, ref _))
                    && !message.Author.IsBot)
                {
                    var controller = scope.ServiceProvider.GetRequiredService<CharacterController>();

                    bool leveledUp = await controller.GiveExperienceAsync(message.Author.Id, message.Channel.Id, message.Content);

                    if (leveledUp)
                        await msg.Author.SendMessageAsync("Level up!");

                    return;
                }
            }
        }
    }
}