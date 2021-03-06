using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Frags.Discord.Modules.Preconditions;
using Frags.Presentation.Controllers;
using Frags.Presentation.ViewModels.Effects;

namespace Frags.Discord.Modules
{
    public class EffectModule : ModuleBase
    {
        private readonly EffectController _controller;

        public EffectModule(EffectController controller)
        {
            _controller = controller;
        }

        [Command("effect create")]
        [Alias("effects create")]
        public async Task CreateEffectAsync([Remainder]string effectName)
        {
            var result = await _controller.CreateEffectAsync(Context.User.Id, effectName);
            await ReplyAsync(result.Message);
        }

        [Command("effect list")]
        [Alias("effects list", "list effect", "list effects")]
        public async Task ListEffectsAsync()
        {
            var result = await _controller.ListCreatedEffectsAsync(Context.User.Id);

            var embed = new EmbedBuilder();
            embed.WithTitle(Context.User.Username + "'s Effects");
            embed.WithDescription(result.Message);
            await ReplyAsync(embed: embed.Build());
        }

        [Command("effect set")]
        [Alias("effects set", "set effect", "set effects")]
        public async Task SetStatisticEffectAsync(string effectName, string statName, int value)
        {
            var result = await _controller.SetStatisticEffectAsync(effectName, statName, value);
            await ReplyAsync(result.Message);
        }

        [Command("effect description")]
        [Alias("effect desc", "effects desc", "effects description")]
        public async Task SetEffectDescriptionAsync(string effectName, [Remainder]string desc)
        {
            var result = await _controller.SetDescriptionAsync(effectName, desc);
            await ReplyAsync(result.Message);
        }

        [Command("effect rename")]
        [Alias("effects rename")]
        public async Task RenameEffectAsync(string effectName, [Remainder]string newName)
        {
            var result = await _controller.RenameEffectAsync(effectName, newName);
            await ReplyAsync(result.Message);
        }

        [Command("effect delete")]
        [Alias("effects delete")]
        public async Task DeleteEffectAsync([Remainder]string effectName)
        {
            var result = await _controller.DeleteEffectAsync(effectName);
            await ReplyAsync(result.Message);
        }
    }

    public class EffectCharacterModule : ModuleBase
    {
        private readonly EffectController _controller;

        public EffectCharacterModule(EffectController controller)
        {
            _controller = controller;
        }

        [Command("effect add")]
        [Alias("effects add", "effect apply", "effects apply", "applyeffect", "apply")]
        public async Task AddEffectAsync([Remainder]string effectName)
        {
            var result = await _controller.AddEffectAsync(Context.User.Id, effectName);
            await ReplyAsync(result.Message);
        }

        [Command("effect remove")]
        [Alias("effects remove", "effect unapply", "effects unapply", "unapplyeffect", "unapply")]
        public async Task RemoveEffectAsync([Remainder]string effectName)
        {
            var result = await _controller.RemoveEffectAsync(Context.User.Id, effectName);
            await ReplyAsync(result.Message);
        }

        [Command("show effects")]
        [Alias("show effect", "effects show", "effect show")]
        public async Task ViewCharacterEffectsAsync(IUser user = null)
        {
            user = user ?? Context.User;
            var result = await _controller.ShowCharacterEffectsAsync(user.Id);

            if (!result.IsSuccess)
            {
                await ReplyAsync(result.Message);
                return;
            }

            StringBuilder output = new StringBuilder();
            var viewModel = (ShowCharacterEffectsViewModel)result.ViewModel;
            
            foreach (var effect in viewModel.Effects)
            {
                output.Append($"__**{effect.Name}:**__ {effect.Description}\n");

                foreach (var statEffect in effect.StatisticEffects)
                {
                    if (statEffect.StatisticValue.Value == 0)
                        continue;

                    output.Append($"{statEffect.Statistic.Name}: {statEffect.StatisticValue.Value}\n");
                }

                output.Append("\n");
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(output.ToString());
            await ReplyAsync(embed: eb.Build());
        }
    }
}