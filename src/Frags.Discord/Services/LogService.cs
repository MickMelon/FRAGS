using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace Frags.Discord.Services
{
    public class LogService
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public LogService(DiscordSocketClient client, CommandService commands)
        {
            _client = client;
            _commands = commands;

            _client.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        /// <summary>
        /// Prints DiscordSocketClient and CommandService logs
        /// to the console.
        /// </summary>
        private Task OnLogAsync(LogMessage msg)
        {
            return Console.Out.WriteLineAsync(msg.ToString());
        }
    }
}