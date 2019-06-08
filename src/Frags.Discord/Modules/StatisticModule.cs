using System.Linq;
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
    [RequireAdminRole]
    public class StatisticModule : ModuleBase
    {
        private readonly StatisticController _statController;

        public StatisticModule(StatisticController statController)
        {
            _statController = statController;
        }

        [Command("seed")]
        public async Task SeedDatabaseAsync()
        {
            await ReplyAsync("Beginning to seed the database.");
            await Services.SeedService.Seed(_statController);
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

    public class StatisticCharacterModule : ModuleBase
    {
        private readonly StatisticController _statController;

        public StatisticCharacterModule(StatisticController statController)
        {
            _statController = statController;
        }

        [Command("check")]
        public async Task CheckStatisticAsync(string statName, int value, IUser user = null)
        {
            user = user ?? Context.User;
            var result = await _statController.CheckStatisticAsync(user.Id, statName, value);
            await ReplyAsync(result.Message);
        }

        [Command("set")]
        public async Task SetStatisticAsync(string statName, int? newValue = null)
        {
            var result = await _statController.SetStatisticAsync(Context.User.Id, statName, newValue);
            await ReplyAsync(result.Message);
        }

        [Command("spend")]
        public async Task UsePointsOnStatisticAsync(string statName, int? newValue = null)
        {
            var result = await _statController.UsePointsOnStatisticAsync(Context.User.Id, statName, newValue);
            await ReplyAsync(result.Message);
        }

        [Command("proficiency")]
        [Alias("proficient", "prof")]
        public async Task SetProficiencyAsync(string statName, bool prof = true)
        {
            var result = await _statController.SetProficiencyAsync(Context.User.Id, statName, prof);
            await ReplyAsync(result.Message);
        }

        [Command("show statistics")]
        [Alias("show stat", "show stats", "statistics show", "stats show", "stat show")]
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
            var viewModel = (ShowCharacterStatisticsViewModel)result.ViewModel;
            
            foreach (var attrib in viewModel.Statistics.Keys.OrderByDescending(x => x.Order))
            {
                // Example: "Strength: 5" or "Strength: N/A"
                output.Append($"__**{attrib.Name}: {attrib.Value?.ToString() ?? "N/A"}**__\n");

                if (attrib.IsProficient.HasValue && attrib.IsProficient.Value)
                    output.Append("*");

                // Loop through associated skills with attribute
                foreach (var skill in viewModel.Statistics[attrib].OrderBy(x => x.Name))
                {
                    // Example: "Powerlifting: 50" or "Powerlifting: N/A"
                    output.Append($"**{skill.Name}:** {skill.Value?.ToString() ?? "N/A"}");

                    if (skill.IsProficient.HasValue && skill.IsProficient.Value)
                        output.Append("*");

                    output.Append("\n");
                }
                output.Append("\n");
            }

            if (viewModel.AttributePoints > 0)
                output.Append($"*You have {viewModel.AttributePoints} attribute points left to spend!*\n");

            if (viewModel.SkillPoints > 0)
                output.Append($"*You have {viewModel.SkillPoints} skill points left to spend!*\n");

            if (!string.IsNullOrWhiteSpace(viewModel.ProgressionInformation))
                output.Append($"Progression info:\n{viewModel.ProgressionInformation}\n");

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle($"{viewModel.CharacterName}'s Statistics");
            eb.WithDescription(output.ToString());
            await ReplyAsync(embed: eb.Build());
        }

        [Command("set")]
        [RequireAdminRole]
        public async Task ForceSetStatisticAsync(IUser user, string statName, int? newValue = null)
        {
            var result = await _statController.SetStatisticAsync(user.Id, statName, newValue, true);
            await ReplyAsync(result.Message);
        }

        [Command("proficiency")]
        [Alias("proficient", "prof")]
        [RequireAdminRole]
        public async Task ForceSetProficiencyAsync(IUser user, string statName, bool prof = true)
        {
            var result = await _statController.SetProficiencyAsync(user.Id, statName, prof, true);
            await ReplyAsync(result.Message);
        }

        [Command("exp")]
        [RequireAdminRole]
        public async Task AddExperienceAsync(IUser user, int xp)
        {
            var result = await _statController.AddExperienceAsync(user.Id, xp);
            await ReplyAsync(result.Message);
        }

        [Command("attributepoints")]
        [Alias("attribpoints", "apoints", "addattributepoints", "addattribpoints", "addapoints")]
        [RequireAdminRole]
        public async Task AddAttributePointsAsync(IUser user, int pts)
        {
            var result = await _statController.AddAttributePointsAsync(user.Id, pts);
            await ReplyAsync(result.Message);
        }

        [Command("skillpoints")]
        [Alias("spoints", "addskillpoints", "addspoints")]
        [RequireAdminRole]
        public async Task AddSkillPointsAsync(IUser user, int pts)
        {
            var result = await _statController.AddSkillPointsAsync(user.Id, pts);
            await ReplyAsync(result.Message);
        }

        [Command("reset")]
        [RequireAdminRole]
        public async Task ResetCharacterAsync(IUser user)
        {
            var result = await _statController.ResetStatisticsAsync(user.Id);
            await ReplyAsync(result.Message);
        }
    }
}