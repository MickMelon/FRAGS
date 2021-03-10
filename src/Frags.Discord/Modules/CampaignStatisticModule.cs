using System.Threading.Tasks;
using Discord.Commands;
using Frags.Discord.Modules.Preconditions;
using Frags.Discord.Services;
using Frags.Presentation.Controllers;

namespace Frags.Discord.Modules
{
    [Group("campaign statistic")]
    [Alias("campaign statistics", "campaign stat", "campaign stats", "camp statistic", "camp statistics", "camp stat", "camp stats")]
    [RequireAdminRole]
    public class CampaignStatisticModule : ModuleBase
    {
        private readonly CampaignStatisticController _campStatController;

        public CampaignStatisticModule(CampaignStatisticController statController)
        {
            _campStatController = statController;
        }

        [Command("create attribute")]
        [Alias("create attrib")]
        public async Task CreateAttributeAsync([Remainder]string statName)
        {
            var result = await _campStatController.CreateCampaignAttributeAsync(statName, Context.User.Id, Context.Channel.Id);
            await ReplyAsync(result.Message);
        }

        [Command("create skill")]
        public async Task CreateSkillAsync(string statName, [Remainder]string attribName)
        {
            var result = await _campStatController.CreateCampaignSkillAsync(statName, attribName, Context.User.Id, Context.Channel.Id);
            await ReplyAsync(result.Message);
        }

        [Command("alias")]
        public async Task AddAliasAsync(string statName, [Remainder]string alias)
        {
            var result = await _campStatController.AddCampaignAliasAsync(statName, alias, Context.User.Id, Context.Channel.Id);
            await ReplyAsync(result.Message);
        }

        [Command("alias clear")]
        [Alias("clearalias", "clearaliases")]
        public async Task ClearAliasesAsync([Remainder]string statName)
        {
            var result = await _campStatController.ClearCampaignAliasesAsync(statName, Context.User.Id, Context.Channel.Id);
            await ReplyAsync(result.Message);
        }

        [Command("rename")]
        public async Task RenameStatisticAsync(string statName, [Remainder]string newName)
        {
            var result = await _campStatController.RenameCampaignStatisticAsync(statName, newName, Context.User.Id, Context.Channel.Id);
            await ReplyAsync(result.Message);
        }

        [Command("delete")]
        public async Task DeleteStatisticAsync([Remainder]string statName)
        {
            var result = await _campStatController.DeleteCampaignStatisticAsync(statName, Context.User.Id, Context.Channel.Id);
            await ReplyAsync(result.Message);
        }
    }
}