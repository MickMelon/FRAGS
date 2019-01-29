using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Frags.Core.Controllers;

namespace Frags.Discord.Modules
{
    public class CharacterModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterController _controller;

        public CharacterModule(CharacterController controller)
        {
            _controller = controller;
        }

        [Command("show")]
        public async Task ShowCharacterAsync()
        {
            ulong discordId = Context.User.Id;
            string result = await _controller.ShowCharacterAsync(discordId);
            await ReplyAsync(result);
        }  

        [Command("roll")]
        public async Task RollAsync(string skill)
        {
            ulong discordId = Context.User.Id;
            string result = await _controller.RollAsync(discordId, skill);
            await ReplyAsync(result);
        }

        [Command("rollagainst")]
        public async Task RollAgainstAsync(IUser targetUser, string skill)
        {
            ulong discordId = Context.User.Id;
            ulong targetDiscordId = targetUser.Id;
            string result = await _controller.RollAgainstAsync(discordId, targetDiscordId, skill);
            await ReplyAsync(result);
        }
    }
}