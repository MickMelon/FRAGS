using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Frags.Core.Common;
using Frags.Presentation.Controllers;

namespace Frags.Discord.Modules
{
    public class RollModule : ModuleBase<SocketCommandContext>
    {
        private readonly RollController _controller;

        public RollModule(RollController controller)
        {
            _controller = controller;
        }

        [Command("roll")]
        [Alias("r")]
        public async Task RollAsync(string stat, params IUser[] users)
        {
            if (users.Length == 0)
            {
                var result = await _controller.RollStatisticAsync(Context.User.Id, stat);
                await ReplyAsync(result.Message);
                return;
            }
            else
            {
                StringBuilder messages = new StringBuilder();

                foreach (var user in users)
                {
                    var result = await _controller.RollStatisticAsync(user.Id, stat);
                    messages.Append(result.Message + "\n");
                }

                await ReplyAsync(messages.ToString());
            }
        }

        [Command("rollmanual")]
        [Alias("rollm", "rm")]
        public async Task RollManualAsync(string stat, int value)
        {
            var result = await _controller.RollStatisticWithValueAsync(Context.User.Id, stat, value, Context.User.Username);
            await ReplyAsync(result.Message);
        }

        [Command("rolldice")]
        [Alias("rd")]
        public async Task RollDiceAsync(int dieCount, int sides, int bonus = 0)
        {
            if (dieCount > 20 || sides > 100)
            {
                await ReplyAsync(Messages.TOO_HIGH);
                return;
            }

            var dice = GameRandom.RollDice(dieCount, sides);

            StringBuilder sb = new StringBuilder();
            for (int die = 0; die < dice.Length; die++)
                sb.Append($"[{dice[die]}] + ");

            sb.Append($"{bonus} = {dice.Sum() + bonus}");

            await ReplyAsync(string.Format(Messages.ROLL_DICE, sb.ToString()));
        }

        [Command("broll")]
        [Alias("br")]
        public async Task EffectsRollAsync(string stat, params IUser[] users)
        {
            if (users.Length == 0)
            {
                var result = await _controller.RollStatisticAsync(Context.User.Id, stat, true);
                await ReplyAsync(result.Message);
                return;
            }
            else
            {
                StringBuilder messages = new StringBuilder();

                foreach (var user in users)
                {
                    var result = await _controller.RollStatisticAsync(user.Id, stat, true);
                    messages.Append(result.Message + "\n");
                }

                await ReplyAsync(messages.ToString());
            }
        }

        [Command("rollagainst")]
        [Alias("rollvs", "rv")]
        public async Task RollAgainstAsync(IUser targetUser, [Remainder]string stat)
        {
            ulong discordId = Context.User.Id;
            ulong targetDiscordId = targetUser.Id;
            var result = await _controller.RollStatisticAgainstAsync(discordId, targetDiscordId, stat);
            await ReplyAsync(result.Message);
        }

        [Command("brollagainst")]
        [Alias("brollvs", "brv")]
        public async Task EffectsRollAgainstAsync(IUser targetUser, [Remainder]string stat)
        {
            ulong discordId = Context.User.Id;
            ulong targetDiscordId = targetUser.Id;
            var result = await _controller.RollStatisticAgainstAsync(discordId, targetDiscordId, stat, true);
            await ReplyAsync(result.Message);
        }
    }
}