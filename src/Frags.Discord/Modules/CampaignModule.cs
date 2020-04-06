using System;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.Commands;
using Frags.Discord.Modules.Preconditions;
using Frags.Core.Common.Extensions;
using Frags.Presentation.Controllers;
using Frags.Presentation.ViewModels.Characters;

namespace Frags.Discord
{
    [Group("campaign")]
    [Alias("camp")]
    public class CampaignModule : InteractiveBase
    {
        private readonly CampaignController _controller;
        private readonly CharacterController _charController;

        public CampaignModule(CampaignController controller, CharacterController charController)
        {
            _controller = controller;
            _charController = charController;
        }

        [Command("convert")]
        public async Task ConvertCharacterToCampaignAsync()
        {
            var showResult = await _charController.ShowCharacterAsync(Context.User.Id);
            if (!showResult.IsSuccess)
            {
                await ReplyAsync(showResult.Message);
                return;
            }

            var view = (ShowCharacterViewModel)showResult.ViewModel;
            var charName = view.Name;

            await ReplyAsync($"This command will permanently convert your active character (\"{charName}\") to the Campaign associated with this channel. To continue please type the name of your character.");

            var nextMessage = await NextMessageAsync(fromSourceUser: true, inSourceChannel: true, timeout: TimeSpan.FromMinutes(1));
            if (!nextMessage.Content.EqualsIgnoreCase(charName))
            {
                return;
            }

            var result = await _controller.ConvertCharacterAsync(Context.User.Id, Context.Channel.Id);
            await ReplyAsync(result.Message);
        }

        [Command("create")]
        public async Task CreateCampaignAsync([Remainder]string name)
        {
            await ReplyAsync((await _controller.CreateCampaignAsync(Context.User.Id, name)).Message);
        }

        [Command("channeladd")]
        [Alias("addchannel", "chanadd", "addchan")]
        [RequireAdminRole]
        public async Task AddChannelToCampaign([Remainder]string name)
        {
            await ReplyAsync((await _controller.AddCampaignChannelAsync(name, Context.Channel.Id)).Message);
        }

        [Command("info")]
        public async Task GetCampaignInfoAsync([Remainder]string name = null)
        {
            Frags.Presentation.Results.IResult result;

            if (name == null)
                result = await _controller.GetCampaignInfoAsync(Context.Channel.Id);
            else
                result = await _controller.GetCampaignInfoAsync(name);

            await ReplyAsync(result.Message);
        }
    }
}