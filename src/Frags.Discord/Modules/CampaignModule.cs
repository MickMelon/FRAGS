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
using Frags.Presentation.ViewModels.Campaigns;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Text;
using Frags.Core.Common;

namespace Frags.Discord
{
    [Group("campaign")]
    [Alias("camp")]
    public class CampaignModule : InteractiveBase
    {
        private readonly CampaignController _controller;

        public CampaignModule(CampaignController controller)
        {
            _controller = controller;
        }

        [Command("convert")]
        public async Task ConvertCharacterToCampaignAsync()
        {
            var result = await _controller.ConvertCharacterAsync(Context.User.Id, Context.Channel.Id);
            await ReplyAsync(result.Message);
        }

        [Command("configure")]
        [Alias("config", "set")]
        public async Task ConfigureCampaignAsync(string property, string value)
        {
            await ReplyAsync((await _controller.ConfigureCampaignAsync(Context.User.Id, Context.Channel.Id, property, (object)value)).Message);
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
            Presentation.Results.IResult result;

            if (name == null)
            {
                result = await _controller.GetCampaignInfoAsync(Context.Channel.Id);
            }
            else
            {
                result = await _controller.GetCampaignInfoAsync(name);
            }

            if (result.ViewModel == null)
            {
                await ReplyAsync(result.Message);
                return;
            }

            var view = (ShowCampaignViewModel) result.ViewModel;

            var embed = new EmbedBuilder();
            
            var chanNameList = new List<string>();
            if (view.Channels != null)
            {
                foreach (var channel in view.Channels)
                {
                    var socketChannel = Context.Client.GetChannel(channel.Id) as SocketGuildChannel;
                    if (socketChannel != null)
                        chanNameList.Add(socketChannel.Name);
                }
            }
            
            string ownerName = "Unknown";
            if (view.Owner != null)
            {
                var socketOwner = Context.Client.GetUser(view.Owner.UserIdentifier);
                if (socketOwner != null)
                    ownerName = socketOwner.Username;
            }

            var characterNames = string.Join(", ", view.CharacterNames);
            var chanNames = string.Join(", ", chanNameList);

            embed.WithTitle(view.Name + " Campaign");
            embed.WithDescription($"**Owner:** {ownerName}\n" +
                $"**Characters:** {(string.IsNullOrWhiteSpace(characterNames) ? "None" : characterNames)}\n" +
                $"**Channels:** {(string.IsNullOrWhiteSpace(chanNames) ? "None" : chanNames)}");
            
            await ReplyAsync(message: Context.User.Mention, embed: embed.Build());
        }

        [Command("options")]
        public async Task GetCampaignOptionsAsync([Remainder]string name = null)
        {
            Presentation.Results.IResult result;

            if (name == null)
                result = await _controller.GetCampaignInfoAsync(Context.Channel.Id);
            else
                result = await _controller.GetCampaignInfoAsync(name);

            var view = (ShowCampaignViewModel) result.ViewModel;
            var embed = new EmbedBuilder();
            
            embed.WithTitle(view.Name + " Campaign");
            string desc = "";

            if (view.StatisticOptions != null)
            {
                StringBuilder statOpts = new StringBuilder("**Statistic Options:**\n");
                foreach (var property in view.StatisticOptions.GetType().GetProperties())
                {
                    if (property.Name == "Id" || property.Name == "ExpEnabledChannels")
                        continue;

                    statOpts.Append($"*{property.Name}:* {property.GetValue(view.StatisticOptions)}\n");
                }
                desc += statOpts;
            }
            
            embed.WithDescription(desc);
            await ReplyAsync(message: Context.User.Mention, embed: embed.Build());
        }
    }
}