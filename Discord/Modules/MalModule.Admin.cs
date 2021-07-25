using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Hamsterland.MyAnimeList.Modules
{
    public partial class MalModule
    {
        [Command("verify")]
        [Summary("Gives the verified role to a user.")]
        [RequireUserPermission(GuildPermission.BanMembers)]
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
        [RequireOwner]
        [Priority(10)]
        public async Task Unlink(IGuildUser user)
        {
            await _accountService.Delete(user.Id);
            await ReplyAsync($"Unliked {user}'s Discord account from their MyAnimeList account.");
        }
        
        [Command("unlink", RunMode = RunMode.Async)]
        [Summary("Unlinks a user's Discord account from their MyAnimeList account.")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        [Priority(5)]
        public async Task UnlinkModerator(IGuildUser user)
        {
            await Unlink(user);
        }
    }
}