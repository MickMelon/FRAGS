using System.Threading.Tasks;
using Discord.Commands;
using Frags.Discord.Modules.Preconditions;
using Frags.Presentation.Controllers;

namespace Frags.Discord.Modules
{
    [Group("statistic")]
    [Alias("stat")]
    public class StatisticModule : ModuleBase
    {
        private readonly CharacterController _charController;
        private readonly StatisticController _statController;

        public StatisticModule(CharacterController charController, StatisticController statController)
        {
            _charController = charController;
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

        [Command("set")]
        public async Task SetAttributeAsync(string statName, int? newValue = null)
        {
            var result = await _statController.SetStatisticAsync(Context.User.Id, statName, newValue);
            await ReplyAsync(result.Message);
        }
    }
}