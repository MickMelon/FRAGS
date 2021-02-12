using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Frags.Presentation.Controllers;
using Frags.Presentation.ViewModels.Effects;

namespace Frags.Discord.Modules
{
    public class EffectCharacterModule : ModuleBase
    {
        private readonly EffectController _controller;

        public EffectCharacterModule(EffectController controller)
        {
            _controller = controller;
        }

        [Command("effect add")]
        [Alias("effect apply", "applyeffect")]
        public async Task AddEffectAsync([Remainder]string effectName)
        {
            var result = await _controller.AddEffectAsync(Context.User.Id, effectName);
            await ReplyAsync(result.Message);
        }

        [Command("effect remove")]
        [Alias("effect unapply", "unapplyeffect")]
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
                    if (statEffect.Value.Value == 0)
                        continue;

                    output.Append($"{statEffect.Key.Name}: {statEffect.Value.Value}\n");
                }

                output.Append("\n");
            }

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithDescription(output.ToString());
            await ReplyAsync(embed: eb.Build());
        }
    }
}