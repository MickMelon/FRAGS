using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Frags.Presentation.Controllers;
using Frags.Presentation.ViewModels;

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
            var result = await _controller.ShowCharacterAsync(discordId);
            
            if (!result.IsSuccess) 
            {
                await ReplyAsync(result.Message);
                return;
            }            

            var view = (ShowCharacterViewModel) result.ViewModel;            
            await ReplyAsync($"{view.Name} | {view.Description} | {view.Story}");
        }  

        [Command("create")]
        public async Task CreateCharacterAsync(string name)
        {
            ulong discordId = Context.User.Id;
            var result = await _controller.CreateCharacterAsync(discordId, name);
            await ReplyAsync(result.Message);
        }
    }
}