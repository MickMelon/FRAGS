using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Frags.Core.Common.Extensions;
using Frags.Core.DataAccess;
using Frags.Core.Game.Progression;
using Frags.Core.Game.Rolling;
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
                .AddDbContext<RpgContext>(opt => opt.UseSqlite("Filename=Frags.db"),
                    contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped)
                .AddTransient<ICharacterProvider, EfCharacterProvider>()
                .AddTransient<IEffectProvider, EfEffectProvider>()
                .AddTransient<IStatisticProvider, EfStatisticProvider>();

        /// <summary>
        /// Adds the game services to the collection.
        /// </summary>
        private static IServiceCollection AddGameServices(IServiceCollection services) =>
            services
                .AddTransient<CharacterController>()
                .AddTransient<EffectController>()
                .AddTransient<RollController>()
                .AddTransient<StatisticController>();

        private static IServiceCollection AddConfiguredServices(IServiceCollection services)
        {
            var configBuilder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile("Config.json", optional: false, reloadOnChange: true);
                
            var jsonConfig = configBuilder.Build();

            return services
                .Configure<GeneralOptions>(jsonConfig.GetSection(nameof(GeneralOptions)))
                .Configure<RollOptions>(jsonConfig.GetSection(nameof(RollOptions)))
                .Configure<StatisticOptions>(jsonConfig.GetSection(nameof(StatisticOptions)))
                .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<GeneralOptions>>().Value)
                .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<RollOptions>>().Value)
                .AddScoped(cfg => cfg.GetService<IOptionsSnapshot<StatisticOptions>>().Value)
                .AddTransient<FragsRollStrategy>()
                .AddTransient<MockRollStrategy>()
                .AddTransient<GenericProgressionStrategy>()
                .AddTransient<NewVegasProgressionStrategy>()
                .AddTransient(provider =>
                    ResolveServices<IRollStrategy>(provider, provider.GetRequiredService<RollOptions>().RollStrategy))
                .AddTransient(provider =>
                    ResolveServices<IProgressionStrategy>(provider, provider.GetRequiredService<StatisticOptions>().ProgressionStrategy));
        }

        private static T ResolveServices<T>(IServiceProvider provider, string typeName)
        {
            var assembly = typeof(T).Assembly;

            // Case-insensitively search the types in the specified assembly
            var type = assembly.ExportedTypes
                .Where(x => typeof(T).IsAssignableFrom(x))
                .Single(x => x.Name.ContainsIgnoreCase(typeName));

            return (T)provider.GetRequiredService(type);
        }
    }
}