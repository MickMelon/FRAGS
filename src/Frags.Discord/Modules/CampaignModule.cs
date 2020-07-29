using System;
using System.Threading.Tasks;
using Discord.Addons.Interactive;
using Discord.Commands;
using Frags.Discord.Modules.Preconditions;
using Frags.Core.Common.Extensions;
using Frags.Database.DataAccess;
using Frags.Presentation.Controllers;
using Frags.Presentation.ViewModels.Characters;
using Frags.Core.DataAccess;

namespace Frags.Discord
{
    [Group("campaign")]
    [Alias("camp")]
    public class CampaignModule : InteractiveBase
    {
        private readonly ICampaignController _controller;
        private readonly CharacterController _charController;

        public CampaignModule(ICampaignController controller, CharacterController charController)
        {
            _controller = controller;
            _charController = charController;
        }

        [Command("convert")]
        public async Task ConvertCharacterToCampaignAsync()
        {
            var result = await _controller.ConvertCharacterAsync(Context.User.Id, Context.Channel.Id);
            await ReplyAsync(result);
        }

        [Command("configure")]
        [Alias("config", "set")]
        public async Task ConfigureCampaignAsync(string property, string value)
        {
            await ReplyAsync(await _controller.ConfigureCampaignAsync(Context.User.Id, Context.Channel.Id, property, (object)value));
        }

        [Command("create")]
        public async Task CreateCampaignAsync([Remainder]string name)
        {
            await ReplyAsync((await _controller.CreateCampaignAsync(Context.User.Id, name)));
        }

        [Command("channeladd")]
        [Alias("addchannel", "chanadd", "addchan")]
        [RequireAdminRole]
        public async Task AddChannelToCampaign([Remainder]string name)
        {
            await ReplyAsync((await _controller.AddCampaignChannelAsync(name, Context.Channel.Id)));
        }

        [Command("info")]
        public async Task GetCampaignInfoAsync([Remainder]string name = null)
        {
            string result;

            if (name == null)
                result = await _controller.GetCampaignInfoAsync(Context.Channel.Id);
            else
                result = await _controller.GetCampaignInfoAsync(name);

            await ReplyAsync(result);
        }
    }
}