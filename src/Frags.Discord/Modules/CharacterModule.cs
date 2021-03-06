using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Frags.Core.Common;
using Frags.Core.Statistics;
using Frags.Discord.Modules.Preconditions;
using Frags.Presentation.Controllers;
using Frags.Presentation.Results;
using Frags.Presentation.ViewModels.Characters;

namespace Frags.Discord.Modules
{
    public class CharacterModule : ModuleBase<SocketCommandContext>
    {
        private readonly CharacterController _controller;
        private static readonly TimeSpan MESSAGE_DELETION_DELAY = TimeSpan.FromSeconds(10);

        public CharacterModule(CharacterController controller)
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
            var embed = new EmbedBuilder();
            embed.WithTitle(view.Name);
            embed.WithDescription($"**Description:** {view.Description}\n" +
                $"**Level:** {view.Level}\n" +
                $"**Money:** {view.Money}\n" +
                $"**Experience:** {view.Experience}\n" +
                $"**Progression:** {view.ProgressionInformation}");
            
            await ReplyAsync(message: Context.User.Mention, embed: embed.Build());
        }

        [Command("list")]
        public async Task ListCharactersAsync()
        {
            var result = await _controller.ListCharactersAsync(Context.User.Id);
            var embed = new EmbedBuilder();
            embed.WithTitle(Context.User.Username + "'s Characters");
            embed.WithDescription(result.Message);

            await ReplyAsync(embed: embed.Build());
        }

        [Command("story")]
        public async Task ShowCharacterStoryAsync(IUser user = null)
        {
            user = user ?? Context.User;
            var result = await _controller.ShowCharacterAsync(user.Id);

            if (!result.IsSuccess)
            {
                await ReplyAsync(result.Message);
                return;
            }

            var view = (ShowCharacterViewModel)result.ViewModel;
            var embed = new EmbedBuilder();
            embed.WithTitle($"{view.Name}'s Story");
            embed.WithDescription(view.Story);

            await ReplyAsync(message: Context.User.Mention, embed: embed.Build());
        }

        [Command("create")]
        public async Task CreateCharacterAsync([Remainder]string name)
        {
            if (name.Length < 3) return;
            if (name.Length > 30) return;

            ulong discordId = Context.User.Id;
            var result = await _controller.CreateCharacterAsync(discordId, name);
            await ReplyAsync(result.Message);
        }

        [Command("changename")]
        public async Task RenameCharacterAsync([Remainder]string newName)
        {
            if (newName.Length < 3) return;
            if (newName.Length > 30) return;

            var result = await _controller.RenameCharacterAsync(Context.User.Id, newName);
            await ReplyAsync(result.Message);
        }

        [Command("description")]
        [Alias("desc")]
        public async Task SetCharacterDescriptionAsync([Remainder]string desc)
        {
            ulong discordId = Context.User.Id;

            if (desc.Length > 1500)
            {
                await ReplyAsync(Messages.TOO_HIGH);
                return;
            }

            var result = await _controller.SetCharacterDescriptionAsync(discordId, desc);
            await ReplyAsync(result.Message);
        }

        [Command("story")]
        public async Task SetCharacterStoryAsync([Remainder]string story)
        {
            ulong discordId = Context.User.Id;

            if (story.Length > 1750)
            {
                await ReplyAsync(Messages.TOO_HIGH);
                return;
            }

            var result = await _controller.SetCharacterStoryAsync(discordId, story);
            await ReplyAsync(result.Message);
        }

        [Command("activate")]
        [Alias("act")]
        public async Task ActivateCharacterAsync([Remainder]string name)
        {
            ulong discordId = Context.User.Id;
            var result = await _controller.ActivateCharacterAsync(discordId, name);

            var message = await ReplyAsync(result.Message);
            await Task.Delay(MESSAGE_DELETION_DELAY);

            try
            {
                await message.DeleteAsync();
            }
            catch (Exception) { }
        }

        [Command("pay")]
        public async Task PayCharacterAsync(IUser user, int money)
        {
            ulong callerId = Context.User.Id;
            ulong targetId = user.Id;
            var result = await _controller.GiveMoneyToOtherAsync(callerId, targetId, money);
            await ReplyAsync(result.Message);
        }

        [Command("money")]
        [RequireAdminRole]
        public async Task AddMoneyAsync(IUser user, int money)
        {
            var result = await _controller.GiveMoneyAsync(user.Id, money);
            await ReplyAsync(result.Message);
        }
    }
}