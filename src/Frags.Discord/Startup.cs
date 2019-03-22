using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Frags.Core.DataAccess;
using Frags.Core.Game.Rolling;
using Frags.Core.Game.Statistics;
using Frags.Core.Statistics;
using Frags.Database;
using Frags.Database.DataAccess;
using Frags.Discord.Services;
using Frags.Presentation.Controllers;
using Microsoft.EntityFrameworkCore;
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

        private static IServiceCollection AddConfiguredServices(IServiceCollection services) =>
            services
                .Configure<StatisticOptions>(x =>
                    {
                        x.ProgressionStrategy = "generic";

                        x.InitialSetupMaxLevel = 1;

                        x.InitialAttributeMin = 1;
                        x.InitialAttributeMax = 10;
                        x.InitialAttributePoints = 40;
                        x.InitialAttributesAtMax = 7;
                        x.InitialAttributesProficient = 0;

                        x.InitialSkillMin = 1;
                        x.InitialSkillMax = 150;
                        x.InitialSkillPoints = 58;
                        x.InitialSkillsAtMax = 13;
                        x.InitialSkillsProficient = 3;
                    })
                .AddTransient(cfg => cfg.GetService<IOptionsSnapshot<StatisticOptions>>().Value)
                .Configure<RollOptions>(x =>
                    {
                        x.RollStrategy = "frags";
                    })
                .AddTransient(cfg => cfg.GetService<IOptionsSnapshot<RollOptions>>().Value)
                .AddTransient<IRollStrategy>(provider => 
                {
                    var assembly = typeof(IRollStrategy).Assembly;
                    var config = provider.GetRequiredService<RollOptions>();

                    // Case-insensitively search the types in the specified assembly
                    var type = assembly.ExportedTypes
                        .Single(x => x.Name.IndexOf(config.RollStrategy, StringComparison.OrdinalIgnoreCase) > -1);

                    return (IRollStrategy)Activator.CreateInstance(type);
                })
                .AddTransient<IProgressionStrategy>(provider => 
                {
                    var assembly = typeof(IProgressionStrategy).Assembly;
                    var config = provider.GetRequiredService<StatisticOptions>();

                    // Case-insensitively search the types in the specified assembly
                    var type = assembly.ExportedTypes
                        .Single(x => x.Name.IndexOf(config.ProgressionStrategy, StringComparison.OrdinalIgnoreCase) > -1);

                    // IStatisticProvider statProvider, StatisticOptions statOptions)
                    return (IProgressionStrategy)Activator.CreateInstance(
                        type, 
                        provider.GetRequiredService<IStatisticProvider>(), 
                        provider.GetRequiredService<StatisticOptions>());
                });
                
    }
}