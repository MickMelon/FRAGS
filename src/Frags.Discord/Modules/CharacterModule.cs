using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Frags.Core.Statistics;
using Frags.Presentation.Controllers;
using Frags.Presentation.ViewModels;

namespace Frags.Discord.Modules
{
    public class CharacterModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterController _controller;

        public CharacterModule(CharacterController controller, DiscordSocketClient client)
        {
            _controller = controller;
        }

        [Command("show")]
        public async Task ShowCharacterAsync(IUser user = null)
        {
            user = user ?? Context.User;
            var result = await _controller.ShowCharacterAsync(user.Id);
            
            if (!result.IsSuccess) 
            {
                await ReplyAsync(result.Message);
                return;
            }            

            var view = (ShowCharacterViewModel) result.ViewModel;            
            await ReplyAsync($"{view.Name} | {view.Description} | {view.Story} | {view.Level}");
        }  

        [Command("create")]
        public async Task CreateCharacterAsync(string name)
        {
            ulong discordId = Context.User.Id;
            var result = await _controller.CreateCharacterAsync(discordId, name);
            await ReplyAsync(result.Message);
        }

        [Command("activate")]
        [Alias("act")]
        public async Task ActivateCharacterAsync(string name)
        {
            ulong discordId = Context.User.Id;
            var result = await _controller.ActivateCharacterAsync(discordId, name);
            await ReplyAsync(result.Message);
        }
    }
}