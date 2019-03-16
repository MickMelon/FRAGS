using System.Threading.Tasks;
using Discord.Commands;
using Frags.Presentation.Controllers;

namespace Frags.Discord.Modules
{
    public class StatisticModule : ModuleBase
    {
        private readonly StatisticController _statController;

        public StatisticModule(StatisticController statController)
        {
            _statController = statController;
        }

        [Command("attrib set")]
        public async Task SetInitialAttributes(string statName, int value)
        {
            var result = await _statController.SetStatisticAsync(Context.User.Id, statName, value);
            await ReplyAsync(result.Message);
        }
    }
}