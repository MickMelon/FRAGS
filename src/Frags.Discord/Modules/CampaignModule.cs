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
using System.Linq;
using Frags.Presentation.ViewModels.Statistics;

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

        [Command("statisticoptions")]
        [Alias("statopts", "statopt", "statop", "statops", "statoptions", "statoption")]
        public async Task ConfigureStatisticOptionsAsync(string property, string value)
        {
            await ReplyAsync((await _controller.ConfigureStatisticOptionsAsync(Context.User.Id, Context.Channel.Id, property, (object)value)).Message);
        }

        [Command("rolloptions")]
        [Alias("rollopts", "rollopt", "rollop", "rollops", "rolloption")]
        public async Task ConfigureRollOptionsAsync(string property, string value)
        {
            await ReplyAsync((await _controller.ConfigureRollOptionsAsync(Context.User.Id, Context.Channel.Id, property, (object)value)).Message);
        }

        [Command("create")]
        public async Task CreateCampaignAsync([Remainder]string name)
        {
            await ReplyAsync((await _controller.CreateCampaignAsync(Context.User.Id, name, Context.Channel.Id)).Message);
        }

        [Command("rename")]
        public async Task RenameCampaignAsync([Remainder]string newName)
        {
            await ReplyAsync((await _controller.RenameCampaignAsync(Context.User.Id, Context.Channel.Id, newName)).Message);
        }

        [Command("channeladd")]
        [Alias("addchannel", "chanadd", "addchan")]
        [RequireAdminRole]
        public async Task AddChannelToCampaign([Remainder]string name)
        {
            await ReplyAsync((await _controller.AddCampaignChannelAsync(name, Context.Channel.Id)).Message);
        }

        [Command("channelremove")]
        [Alias("removechannel", "chanrem", "remchan")]
        [RequireAdminRole]
        public async Task RemoveChannelFromCampaign()
        {
            await ReplyAsync((await _controller.RemoveCampaignChannelAsync(Context.Channel.Id)).Message);
        }

        [Command("show")]
        [Alias("info")]
        public async Task GetCampaignInfoAsync([Remainder]string name = null)
        {
            Presentation.Results.IResult result;

            if (name == null)
                result = await _controller.GetCampaignInfoAsync(Context.Channel.Id);
            else
                result = await _controller.GetCampaignInfoAsync(name);

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

            embed.WithColor(Color.Gold);
            embed.WithTitle(view.Name + " Campaign");
            embed.WithDescription($"**Owner:** {ownerName}\n" +
                $"**Characters:** {(string.IsNullOrWhiteSpace(characterNames) ? "None" : characterNames)}\n" +
                $"**Channels:** {(string.IsNullOrWhiteSpace(chanNames) ? "None" : chanNames)}");
            
            await ReplyAsync(message: Context.User.Mention, embed: embed.Build());
        }

        [Command("show options")]
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
                StringBuilder output = new StringBuilder("**Statistic Options:**\n");
                foreach (var property in view.StatisticOptions.GetType().GetProperties())
                {
                    if (property.Name == nameof(view.StatisticOptions.Id) || property.Name == nameof(view.StatisticOptions.ExpEnabledChannels))
                        continue;

                    output.Append($"*{property.Name}:* {property.GetValue(view.StatisticOptions)}\n");
                }

                output.Append("\n");
                desc += output;
            }

            if (view.RollOptions != null)
            {
                StringBuilder output = new StringBuilder("**Roll Options:**\n");
                foreach (var property in view.RollOptions.GetType().GetProperties())
                {
                    if (property.Name == nameof(view.RollOptions.Id))
                        continue;

                    output.Append($"*{property.Name}:* {property.GetValue(view.RollOptions)}\n");
                }

                output.Append("\n");
                desc += output;
            }
            
            embed.WithDescription(desc);
            embed.WithColor(Color.Gold);
            await ReplyAsync(message: Context.User.Mention, embed: embed.Build());
        }

        [Command("show statistics")]
        [Alias("show statistic", "show stats", "show stat")]
        public async Task ShowCampaignStatisticsAsync([Remainder]string name = null)
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

            if (view.Statistics != null)
            {
                StringBuilder output = new StringBuilder("**Statistics:**\n");
                var attributes = view.Statistics.OfType<ShowAttributeViewModel>();
                var skills = view.Statistics.OfType<ShowSkillViewModel>();

                // Group skills by their associated attribute, sort it out pretty
                foreach (var attrib in attributes.OrderByDescending(x => x.Order))
                {
                    output.Append($"__**{attrib.Name}**__\n");

                    // Loop through associated skills with attribute
                    foreach (var skill in skills.Where(x => x.Attribute.Name == attrib.Name).OrderByDescending(x => x.Order))
                        output.Append($"**{skill.Name}**\n");

                    output.Append("\n");
                }

                desc += output;
            }
            else
            {
                desc = "No Statistics found!";
            }

            embed.WithDescription(desc);
            embed.WithColor(Color.Gold);
            await ReplyAsync(message: Context.User.Mention, embed: embed.Build());
        }
    }
}