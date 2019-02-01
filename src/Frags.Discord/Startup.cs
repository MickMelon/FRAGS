using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Frags.Core.Controllers;
using Frags.Core.DataAccess;
using Frags.Discord.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Frags.Discord
{
    internal class Startup
    {
        public async Task StartAsync()
        {
            var services = BuildServiceProvider();

            services.GetRequiredService<LogService>();
            var client = services.GetRequiredService<DiscordSocketClient>();
            var commands = services.GetRequiredService<CommandHandler>();
                      
            await commands.InitializeAsync();          
            await client.LoginAsync(TokenType.Bot, "NTM4ODAwMDYyMTg1MjA5ODYw.Dy5EQg.mJ0KYs5tJOaVa2n_fne8VtaneNs");
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private IServiceProvider BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
                
            services = AddDiscordServices(services);
            services = AddDatabaseServices(services);
            services = AddGameServices(services);

            return services.BuildServiceProvider();
        } 

        private IServiceCollection AddDiscordServices(IServiceCollection services) =>
            services
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    CaseSensitiveCommands = false,
                    DefaultRunMode = RunMode.Async
                }))
                .AddSingleton<CommandHandler>()
                .AddSingleton<LogService>();

        private IServiceCollection AddDatabaseServices(IServiceCollection services) =>
            services
                .AddTransient<ICharacterProvider, MockCharacterProvider>();

        private IServiceCollection AddGameServices(IServiceCollection services) =>
            services
                .AddSingleton<CharacterController>()
                .AddSingleton<RollController>();
    }
}