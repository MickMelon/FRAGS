using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Frags.Discord.Modules.Preconditions;
using Frags.Presentation.Controllers;
using Frags.Presentation.ViewModels.Effects;

namespace Frags.Discord.Modules
{
    [Group("effect")]
    public class EffectModule : ModuleBase
    {
        private readonly EffectController _controller;

        public EffectModule(EffectController controller)
        {
            _controller = controller;
        }

        [Command("create")]
        public async Task CreateEffectAsync([Remainder]string effectName)
        {
            var result = await _controller.CreateEffectAsync(Context.User.Id, effectName);
            await ReplyAsync(result.Message);
        }

        [Command("list")]
        public async Task ListEffectsAsync()
        {
            var result = await _controller.ListCreatedEffectsAsync(Context.User.Id);

            var embed = new EmbedBuilder();
            embed.WithTitle(Context.User.Username + "'s Effects");
            embed.WithDescription(result.Message);
            await ReplyAsync(embed: embed.Build());
        }

        [Command("set")]
        public async Task SetStatisticEffectAsync(string effectName, string statName, int value)
        {
            var result = await _controller.SetStatisticEffectAsync(effectName, statName, value);
            await ReplyAsync(result.Message);
        }

        [Command("description")]
        [Alias("desc")]
        public async Task SetEffectDescriptionAsync(string effectName, [Remainder]string desc)
        {
            var result = await _controller.SetDescriptionAsync(effectName, desc);
            await ReplyAsync(result.Message);
        }

        [Command("rename")]
        public async Task RenameEffectAsync(string effectName, [Remainder]string newName)
        {
            var result = await _controller.RenameEffectAsync(effectName, newName);
            await ReplyAsync(result.Message);
        }

        [Command("delete")]
        public async Task DeleteEffectAsync([Remainder]string effectName)
        {
            var result = await _controller.DeleteEffectAsync(effectName);
            await ReplyAsync(result.Message);
        }
    }

    
}