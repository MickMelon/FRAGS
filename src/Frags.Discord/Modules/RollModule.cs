using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Frags.Presentation.Controllers;

namespace Frags.Discord.Modules
{
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        private readonly RollController _controller;

        public RollModule(RollController controller)
        {
            _controller = controller;
        }

        [Command("roll")]
        public async Task RollAsync(string stat, IUser user = null)
        {
            ulong discordId = user?.Id ?? Context.User.Id;
            var result = await _controller.RollStatisticAsync(discordId, stat);
            await ReplyAsync(result.Message);
        }

        [Command("broll")]
        public async Task EffectsRollAsync(string stat, IUser user = null)
        {
            ulong discordId = user?.Id ?? Context.User.Id;
            var result = await _controller.RollStatisticAsync(discordId, stat, true);
            await ReplyAsync(result.Message);
        }

        [Command("rollagainst")]
        public async Task RollAgainstAsync(IUser targetUser, string stat)
        {
            ulong discordId = Context.User.Id;
            ulong targetDiscordId = targetUser.Id;
            var result = await _controller.RollStatisticAgainstAsync(discordId, targetDiscordId, stat);
            await ReplyAsync(result.Message);
        }

        [Command("brollagainst")]
        public async Task EffectsRollAgainstAsync(IUser targetUser, string stat)
        {
            ulong discordId = Context.User.Id;
            ulong targetDiscordId = targetUser.Id;
            var result = await _controller.RollStatisticAgainstAsync(discordId, targetDiscordId, stat, true);
            await ReplyAsync(result.Message);
        }
    }
}