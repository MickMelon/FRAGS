using System.Threading.Tasks;
using Discord.Commands;
using Frags.Discord.Modules.Preconditions;
using Frags.Presentation.Controllers;

namespace Frags.Discord
{
    [Group("campaign")]
    [Alias("camp")]
    public class CampaignModule : ModuleBase
    {
        private readonly CampaignController _controller;

        public CampaignModule(CampaignController controller)
        {
            _controller = controller;
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