using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Frags.Discord.Modules.Preconditions;
using Frags.Presentation.Controllers;
using Frags.Presentation.ViewModels.Statistics;

namespace Frags.Discord.Modules
{
    [Group("statistic")]
    [Alias("stat", "stats")]
    [RequireRole("FragsAdmin")]
    public class StatisticModule : ModuleBase
    {
        private readonly StatisticController _statController;

        public StatisticModule(StatisticController statController)
        {
            _statController = statController;
        }

        [Command("create attribute")]
        [Alias("create attrib")]
        public async Task CreateAttributeAsync(string statName)
        {
            var result = await _statController.CreateAttributeAsync(statName);
            await ReplyAsync(result.Message);
        }

        [Command("create skill")]
        public async Task CreateSkillAsync(string statName, string attribName)
        {
            var result = await _statController.CreateSkillAsync(statName, attribName);
            await ReplyAsync(result.Message);
        }

        [Command("alias")]
        public async Task AddAliasAsync(string statName, string alias)
        {
            var result = await _statController.AddAliasAsync(statName, alias);
            await ReplyAsync(result.Message);
        }

        [Command("alias clear")]
        [Alias("clearalias", "clearaliases")]
        public async Task ClearAliasesAsync(string statName)
        {
            var result = await _statController.ClearAliasesAsync(statName);
            await ReplyAsync(result.Message);
        }

        [Command("rename")]
        public async Task RenameStatisticAsync(string statName, string newName)
        {
            var result = await _statController.RenameStatisticAsync(statName, newName);
            await ReplyAsync(result.Message);
        }

        [Command("delete")]
        public async Task SetStatisticAsync(string statName)
        {
            var result = await _statController.DeleteStatisticAsync(statName);
            await ReplyAsync(result.Message);
        }
    }

    public class StatisticCharacterModule : ModuleBase
    {
        private readonly StatisticController _statController;

        public StatisticCharacterModule(StatisticController statController)
        {
            _statController = statController;
        }

        [Command("set")]
        public async Task SetStatisticAsync(string statName, int? newValue = null)
        {
            var result = await _statController.SetStatisticAsync(Context.User.Id, statName, newValue);
            await ReplyAsync(result.Message);
        }

        [Command("show statistics")]
        [Alias("show stat", "show stats")]
        public async Task ViewStatisticsAsync(IUser user = null)
        {
            user = user ?? Context.User;
            var result = await _statController.ShowStatisticsAsync(user.Id);

            if (!result.IsSuccess)
            {
                await ReplyAsync(result.Message);
                return;
            }

            StringBuilder output = new StringBuilder();
            var viewModel = (ShowStatisticListViewModel)result.ViewModel;
            
            foreach (var attrib in viewModel.Statistics.Keys)
            {
                // "Strength: 5" or "Strength: N/A"
                output.Append($"__**{attrib.Name}: {attrib.Value?.ToString() ?? "N/A"}**__\n");

                // Loop through associated skills with attribute
                foreach (var skill in viewModel.Statistics[attrib])
                {
                    // "Powerlifting: 50" or "Powerlifting: N/A"
                    output.Append($"**{skill.Name}:** {skill.Value?.ToString() ?? "N/A"}");

                    if (skill.IsProficient.HasValue && skill.IsProficient.Value)
                        output.Append("*");

                    output.Append("\n");
                }
                output.Append("\n");
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(output.ToString());
            await ReplyAsync(embed: eb.Build());
        }
    }
}