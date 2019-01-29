using System.Threading.Tasks;
using Discord.Commands;

namespace Frags.Discord.Modules
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task TestAsync() => 
            await Context.Channel.SendMessageAsync("Test");
    }
}