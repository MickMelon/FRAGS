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
        public async Task RollAsync(string stat)
        {
            ulong discordId = Context.User.Id;
            var result = await _controller.RollStatisticAsync(discordId, stat);
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
    }
}