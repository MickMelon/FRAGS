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
        public async Task CreateEffectAsync(string effectName)
        {
            var result = await _controller.CreateEffectAsync(effectName);
            await ReplyAsync(result.Message);
        }

        [Command("description")]
        public async Task SetEffectDescriptionAsync(string effectName, [Remainder]string desc)
        {
            var result = await _controller.SetDescriptionAsync(effectName, desc);
            await ReplyAsync(result.Message);
        }

        [Command("rename")]
        public async Task RenameEffectAsync(string effectName, string newName)
        {
            var result = await _controller.RenameEffectAsync(effectName, newName);
            await ReplyAsync(result.Message);
        }

        [Command("delete")]
        public async Task DeleteEffectAsync(string effectName)
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
        [Alias("effect apply", "applyeffect")]
        public async Task AddEffectAsync(string effectName)
        {
            var result = await _controller.AddEffectAsync(Context.User.Id, effectName);
            await ReplyAsync(result.Message);
        }

        [Command("effect remove")]
        [Alias("effect unapply", "unapplyeffect")]
        public async Task RemoveEffectAsync(string effectName)
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
                output.Append($"__**{effect.Name}:**__: {effect.Description}\n");

                foreach (var statEffect in effect.StatisticEffects)
                    output.Append($"{statEffect.Statistic.Name}: {statEffect.StatisticValue.Value}\n");

                output.Append("\n");
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(output.ToString());
            await ReplyAsync(embed: eb.Build());
        }
    }
}