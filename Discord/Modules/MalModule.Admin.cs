using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Hamsterland.MyAnimeList.Modules
{
    public class RequireBotOwnerOrPermissionAttribute : PreconditionAttribute
    {
        public RequireBotOwnerOrPermissionAttribute(GuildPermission guildPermission)
        {
            GuildPermission = guildPermission;
        }

        public GuildPermission GuildPermission { get; }
        
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var user = context.User as IGuildUser;

            if (user.Id == 330746772378877954)
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            if (user.GuildPermissions.Has(GuildPermission))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError("No!"));
        }
    }

    public partial class MalModule
    {
        [Command("verify")]
        [Summary("Gives the verified role to a user.")]
        [RequireBotOwnerOrPermission(GuildPermission.BanMembers)]
        public async Task Verify(IGuildUser user)
        {
            if (user.RoleIds.Contains(_verifiedRoleId))
            {
                await ReplyAsync("This user already has the verified role.");
                return;
            }

            await user.AddRoleAsync(_verifiedRoleId);
            await ReplyAsync($"Given the verified role to this {user}");
        }

        [Command("unlink", RunMode = RunMode.Async)]
        [Summary("Unlinks a user's Discord account from their MyAnimeList account.")]
        [RequireBotOwnerOrPermission(GuildPermission.BanMembers)]
        public async Task Unlink(IUser user)
        {
            await _accountService.Delete(user.Id);
            await ReplyAsync($"Unliked {user}'s Discord account from their MyAnimeList account.");
        }
    }
}