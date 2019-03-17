using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Frags.Core.Common;

namespace Frags.Discord.Modules.Preconditions
{
    public class RequireRoleAttribute : PreconditionAttribute
    {
        private readonly string _name;

        public RequireRoleAttribute(string name) => _name = name;

        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is SocketGuildUser user)
            {
                if (user.Roles.Any(r => r.Name == _name))
                    return Task.FromResult(PreconditionResult.FromSuccess());
                
                return Task.FromResult(PreconditionResult.FromError(String.Format(Messages.REQUIRE_ROLE_FAIL, _name)));
            }
            else
                return Task.FromResult(PreconditionResult.FromError(Messages.NOT_IN_GUILD));
        }
    }
}