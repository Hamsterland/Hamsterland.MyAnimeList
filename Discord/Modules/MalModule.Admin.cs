using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace Hamsterland.MyAnimeList.Modules
{
    public partial class MalModule
    {
        [Command("unlink", RunMode = RunMode.Async)]
        [Summary("Unlinks a user's Discord account from their MyAnimeList account.")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        public async Task Unlink(IGuildUser user)
        {
            await _accountService.Delete(user.Id);
            await ReplyAsync($"Unliked {user}'s Discord account from their MyAnimeList account.");
        }
    }
}