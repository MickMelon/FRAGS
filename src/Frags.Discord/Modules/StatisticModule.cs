using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Frags.Discord.Modules.Preconditions;
using Frags.Discord.Services;
using Frags.Presentation.Controllers;
using Frags.Presentation.ViewModels.Statistics;

namespace Frags.Discord.Modules
{
    [Group("statistic")]
    [Alias("statistics", "stat", "stats")]
    [RequireAdminRole]
    public class StatisticModule : ModuleBase
    {
        private readonly StatisticController _statController;
        private readonly SeedService _seedService;

        public StatisticModule(StatisticController statController, SeedService seedService)
        {
            _statController = statController;
            _seedService = seedService;
        }

        [Command("seed")]
        public async Task SeedDatabaseAsync()
        {
            await ReplyAsync("Beginning to seed the database.");
            await _seedService.Seed();
            await ReplyAsync("Completed seeding the database.");
        }

        [Command("create attribute")]
        [Alias("create attrib")]
        public async Task CreateAttributeAsync([Remainder]string statName)
        {
            var result = await _statController.CreateAttributeAsync(statName);
            await ReplyAsync(result.Message);
        }

        [Command("create skill")]
        public async Task CreateSkillAsync(string statName, [Remainder]string attribName)
        {
            var result = await _statController.CreateSkillAsync(statName, attribName);
            await ReplyAsync(result.Message);
        }

        [Command("alias")]
        public async Task AddAliasAsync(string statName, [Remainder]string alias)
        {
            var result = await _statController.AddAliasAsync(statName, alias);
            await ReplyAsync(result.Message);
        }

        [Command("alias clear")]
        [Alias("clearalias", "clearaliases")]
        public async Task ClearAliasesAsync([Remainder]string statName)
        {
            var result = await _statController.ClearAliasesAsync(statName);
            await ReplyAsync(result.Message);
        }

        [Command("order")]
        public async Task OrderStatisticAsync(int order, [Remainder]string statName)
        {
            var result = await _statController.OrderStatisticAsync(statName, order);
            await ReplyAsync(result.Message);
        }

        [Command("rename")]
        public async Task RenameStatisticAsync(string statName, [Remainder]string newName)
        {
            var result = await _statController.RenameStatisticAsync(statName, newName);
            await ReplyAsync(result.Message);
        }

        [Command("delete")]
        public async Task DeleteStatisticAsync([Remainder]string statName)
        {
            var result = await _statController.DeleteStatisticAsync(statName);
            await ReplyAsync(result.Message);
        }
    }
}