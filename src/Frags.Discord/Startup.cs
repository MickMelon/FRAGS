using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Rolling;
using Frags.Core.Game.Statistics;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.DataAccess;
using Frags.Discord.Services;
using Frags.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Frags.Discord
{
    /// <summary>
    /// Used to start the Discord bot.
    /// </summary>
    internal class Startup
    {
        /// <summary>
        /// Sets up the services and starts the bot.
        /// </summary>
        public async Task StartAsync()
        {
            var services = BuildServiceProvider();

            services.GetRequiredService<LogService>();
            var client = services.GetRequiredService<DiscordSocketClient>();
            var commands = services.GetRequiredService<CommandHandler>();
                      
            await commands.InitializeAsync();          
            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DiscordToken"));
            await client.StartAsync();

            await Task.Delay(-1);
        }

        /// <summary>
        /// Builds the service provider.
        /// </summary>
        /// <returns>The built service provider.</returns>
        private static IServiceProvider BuildServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();
                
            services = AddDiscordServices(services);
            services = AddDatabaseServices(services);
            services = AddGameServices(services);
            services = AddConfiguredServices(services);

            // Frags.Core.Game.Rolling.FragsRollStrategy
            //Console.WriteLine(typeof(FragsRollStrategy).ToString());

            // Frags.Core.Game.Rolling.FragsRollStrategy, Frags.Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
            //Console.WriteLine(typeof(FragsRollStrategy).AssemblyQualifiedName);
            
            // Frags.Core.Game.Statistics.GenericProgressionStrategy
            //Console.WriteLine(typeof(GenericProgressionStrategy).ToString());

            return services.BuildServiceProvider();
        } 

        /// <summary>
        /// Adds the Discord services to the collection..
        /// </summary>
        private static IServiceCollection AddDiscordServices(IServiceCollection services) =>
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

        /// <summary>
        /// Adds the database services to the collection.
        /// </summary>
        private static IServiceCollection AddDatabaseServices(IServiceCollection services) =>
            services
                .AddDbContext<RpgContext>(opt => opt.UseInMemoryDatabase("TestDb"), contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped)
                .AddTransient<ICharacterProvider, EfCharacterProvider>()
                .AddTransient<IStatisticProvider, EfStatisticProvider>();

        /// <summary>
        /// Adds the game services to the collection.
        /// </summary>
        private static IServiceCollection AddGameServices(IServiceCollection services) =>
            services
                .AddTransient<CharacterController>()
                .AddTransient<RollController>()
                .AddTransient<StatisticController>();

        private static IServiceCollection AddConfiguredServices(IServiceCollection services)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config.json", optional: false, reloadOnChange: true);
                
            var jsonConfig = configBuilder.Build();

            return services
                .Configure<StatisticOptions>(jsonConfig.GetSection("StatisticOptions"))
                .Configure<RollOptions>(jsonConfig.GetSection("RollOptions"))
                .AddTransient(cfg => cfg.GetService<IOptionsSnapshot<StatisticOptions>>().Value)
                .AddTransient(cfg => cfg.GetService<IOptionsSnapshot<RollOptions>>().Value)
                .AddTransient<IRollStrategy>(provider => 
                {
                    var assembly = typeof(IRollStrategy).Assembly;
                    var config = provider.GetRequiredService<RollOptions>();

                    // Case-insensitively search the types in the specified assembly
                    // TODO: filter types to make sure they inherit from IRollStrategy
                    var type = assembly.ExportedTypes
                        .Single(x => x.Name.EqualsIgnoreCase(config.RollStrategy));

                    return (IRollStrategy)Activator.CreateInstance(type);
                })
                .AddTransient<IProgressionStrategy>(provider => 
                {
                    var assembly = typeof(IProgressionStrategy).Assembly;
                    var config = provider.GetRequiredService<StatisticOptions>();

                    // Case-insensitively search the types in the specified assembly
                    var type = assembly.ExportedTypes
                        .Single(x => x.Name.EqualsIgnoreCase(config.ProgressionStrategy));

                    // IStatisticProvider statProvider, StatisticOptions statOptions)
                    return (IProgressionStrategy)Activator.CreateInstance(
                        type, 
                        provider.GetRequiredService<IStatisticProvider>(), 
                        provider.GetRequiredService<StatisticOptions>());
                });
        }
    }
}