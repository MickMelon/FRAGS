using System.Threading.Tasks;
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
    }
}