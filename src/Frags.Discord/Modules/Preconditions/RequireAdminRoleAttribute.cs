using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Frags.Core.Common;

namespace Frags.Discord.Modules.Preconditions
{
    public class RequireAdminRoleAttribute : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (context.User is SocketGuildUser user)
            {
                var options = services.GetService(typeof(GeneralOptions)) as GeneralOptions;
                if (options == null)
                    return Task.FromResult(PreconditionResult.FromError("Admin role not configured."));

                if (user.Roles.Any(r => r.Name == options.AdminRole))
                    return Task.FromResult(PreconditionResult.FromSuccess());
                
                return Task.FromResult(PreconditionResult.FromError(String.Format(Messages.REQUIRE_ROLE_FAIL, options.AdminRole)));
            }
            else
                return Task.FromResult(PreconditionResult.FromError(Messages.NOT_IN_GUILD));
        }
    }
}