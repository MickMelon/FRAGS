using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Discord;
using Discord.Addons.Interactive;
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
            var context = services.GetRequiredService<RpgContext>();

            if (context.Database.IsSqlite())
                context.Database.EnsureCreated();
                      
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
            services = AddPluginServices(services);
            services = AddConfiguredServices(services);
            services = AddDatabaseServices(services);
            services = AddGameServices(services);

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
                .AddScoped<InteractiveService>()
                .AddSingleton<CommandHandler>()
                .AddSingleton<LogService>()
                .AddSingleton<ReliabilityService>();

        /// <summary>
        /// Adds the database services to the collection.
        /// </summary>
        private static IServiceCollection AddDatabaseServices(IServiceCollection services) =>
            services
                .AddDbContext<RpgContext>(contextLifetime: ServiceLifetime.Scoped, optionsLifetime: ServiceLifetime.Scoped)
                .AddTransient<ICampaignProvider, EfCampaignProvider>()
                .AddTransient<IUserProvider, EfUserProvider>()
                .AddTransient<ICharacterProvider, EfCharacterProvider>()
                .AddTransient<IEffectProvider, EfEffectProvider>()
                .AddTransient<IStatisticProvider, EfStatisticProvider>();

        /// <summary>
        /// Adds the game services to the collection.
        /// </summary>
        private static IServiceCollection AddGameServices(IServiceCollection services) =>
            services
                .AddTransient<SeedService>()
                .AddTransient<CampaignController>()
                .AddTransient<CharacterController>()
                .AddTransient<EffectController>()
                .AddTransient<RollController>()
                .AddTransient<StatisticController>()
                .AddTransient<StatisticCharacterController>();

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
                .AddScoped<FragsRollStrategy>()
                .AddScoped<MockRollStrategy>()
                .AddScoped<GenericProgressionStrategy>()
                .AddScoped<MockProgressionStrategy>()
                .AddScoped<NewVegasProgressionStrategy>()
                .AddScoped(provider =>
                    ResolveServices<IRollStrategy>(provider, provider.GetRequiredService<RollOptions>().RollStrategy))
                .AddScoped(provider =>
                    ResolveServices<IProgressionStrategy>(provider, provider.GetRequiredService<StatisticOptions>().ProgressionStrategy))
                .AddScoped<List<IRollStrategy>>(provider => provider.GetServices<IRollStrategy>().ToList())
                .AddScoped<List<IProgressionStrategy>>(provider => provider.GetServices<IProgressionStrategy>().ToList());
        }

        private static T ResolveServices<T>(IServiceProvider provider, string typeName)
        {
            // Search plugins & the interface's assembly's types
            var type = _pluginTypes.Union(typeof(T).Assembly.ExportedTypes)
                .Where(x => typeof(T).IsAssignableFrom(x))
                .Single(x => x.Name.ContainsIgnoreCase(typeName));

            return (T)provider.GetRequiredService(type);
        }

        // private static List<T> AddServiceLists<T>(IServiceProvider provider)
        // {
            // Console.WriteLine("Running AddServiceLists");

            // var theInterface = typeof(T);

            // // Search plugins & the interface's assembly's types
            // var interfaceAssemblyTypes = typeof(T).Assembly.ExportedTypes;

            // var matchingTypes = _pluginTypes.Union(interfaceAssemblyTypes).Where(type => theInterface.IsAssignableFrom(type));

            // var list = new List<T>();

            // foreach (var type in matchingTypes)
            // {
            //     if (type.Equals(theInterface))
            //         continue;

            //     list.Add((T)provider.GetRequiredService(type));
            // }

            // return list;
        // }

        private static readonly List<Type> _pluginTypes = new List<Type>();

        private static IServiceCollection AddPluginServices(IServiceCollection services)
        {
            List<Assembly> assemblies = new List<Assembly>();

            // Add all .dll's in the Plugins folder the list of assemblies
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Plugins");

            foreach (string dll in Directory.GetFiles(path, "*.dll"))
                assemblies.Add(Assembly.LoadFile(dll));

            // Add all of the .dll's types to a list
            foreach (var assembly in assemblies)
                _pluginTypes.AddRange(assembly.ExportedTypes);

            foreach (var type in _pluginTypes)
                services.AddTransient(type);

            return services;
        }
    }
}