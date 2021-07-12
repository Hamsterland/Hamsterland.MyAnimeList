using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hamsterland.MyAnimeList.Contexts;
using Hamsterland.MyAnimeList.Services.Accounts;
using Hamsterland.MyAnimeList.Services.Activity;
using Hamsterland.MyAnimeList.Services.MyAnimeList;
using HonkSharp.Fluency;
using Interactivity;
using MAL.Downloaders;
using MAL.Factories;
using MAL.Pages;
using MAL.Parsers;
using MoreLinq;
using Uchuumaru.Services.MyAnimeList;

namespace Hamsterland.MyAnimeList.Modules
{
    [Name("MyAnimeList")]
    [Group("mal")]
    [Summary("MyAnimeList-related commands.")]
    public partial class MalModule : ModuleBase<HamsterlandCommandContext>
    {
        private readonly IMalWebService _malWebService;
        private readonly IActivityService _activityService;
        private readonly IAccountService _accountService;
        private readonly InteractivityService _interactivityService;

        public MalModule(
            IMalWebService malWebService,
            IActivityService activityService,
            IAccountService accountService,
            InteractivityService interactivityService)
        {
            _malWebService = malWebService;
            _activityService = activityService;
            _accountService = accountService;
            _interactivityService = interactivityService;
        }

        private readonly ProfileDownloader _profileDownloader = new();
        private readonly CommentsDownloader _commentsDownloader = new();
        private readonly UsersDownloader _usersDownloader = new();

        [Command(RunMode = RunMode.Async)]
        [Summary("Searches for a MyAnimeList account linked to a Discord account.")]
        public async Task Search(IUser user = null)
        {
            user ??= Context.User;
            
            // ReSharper disable once UseDeconstruction
            var account = await _accountService.GetByUserId(user.Id);

            if (account?.MalId is null)
            {
                await ReplyAsync($"{user} does not have a MyAnimeList account linked.");
                return;
            }

            await SendProfile(account.MalId.Value);
        }
        
        [Command("search", RunMode = RunMode.Async)]
        [Summary("Searches for a MyAnimeList account by its username.")]
        public async Task Search([Remainder] string username)
        {
            if (username.Any(char.IsWhiteSpace))
            {
                await ReplyAsync("MAL usernames cannot contain whitespace!");
                return;
            }
            
            if (!await _malWebService.DoesAccountExist(username))
            {
                await SendSimilarUsernames(username);
                return;
            }
            
            await SendProfile(username);
        }

        private async Task SendSimilarUsernames(string username)
        {
            var usersPage = await _usersDownloader.DownloadUsersDocument(username);
            var usernames = usersPage.SimilarUsernames;

            if (usernames.Length == 0)
            {
                await ReplyAsync("Nothing here but chickens...");
                return;
            }
            
            var builder = new EmbedBuilder()
                .WithColor(Constants.DefaultColour)
                .WithTitle("I couldn't find an exact match");

            var columns = usernames
                .Batch(5)
                .ToList();

            if (columns.Count >= 1)
            {
                builder.AddField("Did you mean", CreateColumn(columns[0]), true);
            }

            if (columns.Count >= 2)
            {
                builder.AddField("Or perhaps", CreateColumn(columns[1]), true);
            }

            if (columns.Count >= 3)
            {
                builder.AddField("Maybe?", CreateColumn(columns[2]), true);
            }
            
            builder.AddField("Still no luck?", $"Search for more users [on the website](https://myanimelist.net/users.php?q={username}&cat=user)",true);
            
            await ReplyAsync(embed: builder.Build());
        }

        private static string CreateColumn(IEnumerable<string> usernames)
        {
            var sb = new StringBuilder();

            foreach (var username in usernames)
            {
                sb.AppendLine($@"[`{username}`](https://myanimelist.net/profile/{username})");
            }

            return sb.ToString();
        }
        
        private async Task SendProfile(string username)
        {
            var profilePage = await _profileDownloader.DownloadProfileDocument(username);
            await SendProfile(profilePage);
        }
        
        private async Task SendProfile(int id)
        {
            var commentsPage = await _commentsDownloader.DownloadCommentsDocument(id);
            var profilePage = await _profileDownloader.DownloadProfileDocument(commentsPage.GetUsername());
            await SendProfile(profilePage);
        }

        private async Task SendProfile(ProfilePage profilePage)
        {
            var profile = new ProfileFactory(profilePage).BuildFullProfile();
            var profileEmbed = new ProfileEmbedFactory(profile).BuildFullEmbed();
            await ReplyAsync(embed: profileEmbed);
        }
    }
}