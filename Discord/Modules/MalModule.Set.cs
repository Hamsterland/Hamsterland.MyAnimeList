using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Hamsterland.MyAnimeList.Services.Verification;
using MAL.Parsers;

namespace Hamsterland.MyAnimeList.Modules
{
    public partial class MalModule
    {
        private readonly Verifier _verifier = new();
        private readonly Random _random = new();
        private readonly Emote _loading = Emote.Parse("<a:loading:818260297118384178>");
        private readonly Emote _redSparkle = Emote.Parse("<a:tl1SparklesRed:807192797579837440>");
        private readonly Emote _greenSparkle = Emote.Parse("<a:tl1SparklesGreen:807191823718023168>");
        private const string _failed = "Verification Failed";
        private const string _succeeded = "Verification Succeeded";
        private const ulong _verifiedRoleId = 372178027926519810;
        private const int _maxRetries = 8;
        private const int _retryWaitPeriod = 15000;
        private readonly TimeSpan _totalTime = TimeSpan.FromMilliseconds(_maxRetries * _retryWaitPeriod);

        [Command("set", RunMode = RunMode.Async)]
        [Summary("Links your Discord account to a MyAnimeList account.")]
        public async Task Set(string username)
        {
            if ((await _accountService.GetByUserId(Context.User.Id))?.MalId is not null)
            {
                await ReplyAsync("You already have a MyAnimeList account linked to your Discord account. Please contact Modmail for further help.");
                return;
            }
            
            if (username.ToLower() == "username")
            {
                await ReplyAsync("Are you sure your MAL account is called \"username\"? Replace this with your actual MAL username.");
                return;
            }

            var builder = new EmbedBuilder()
                .WithColor(Constants.DefaultColour)
                .WithUserAsAuthor(Context.User);
            
            var profilePage = await _profileDownloader.DownloadProfileDocument(username);
            
            if (profilePage is null)
            {
                builder
                    .WithColor(Color.Red)
                    .WithTitle(_failed)
                    .WithDescription($"Account {username} does not exist.");

                await ReplyAsync(embed: builder.Build());
                return;
            }
            
            if (profilePage.GetImageUrl() is not null)
            {
                var id = profilePage.GetUserId();

                if (await _accountService.GetByMalId(id) is not null)
                {
                    await ReplyAsync("Another user has already linked this MyAnimeList account to their Discord. Please contact Modmail for further help.");
                    return;
                }
            }
            
            var roleIds = (Context.User as IGuildUser).RoleIds.ToList();
            
            if (!roleIds.Contains(_verifiedRoleId))
            {
                var results = await _verifier.Verify(username, _activityService);
                
                if (results.Any(x => !x.IsSuccess))
                {
                    builder
                        .WithColor(Color.Red)
                        .WithTitle(_failed);
                
                    var sb = new StringBuilder();
                    sb.AppendLine("```diff");
                    
                    foreach (var result in results)
                    {
                        if (!result.IsSuccess)
                        {
                            sb.AppendLine($"- {result.Type}");

                            if (result.Type == VerifierResultType.Image)
                            {
                                builder.WithImageUrl(result.ImageUrl);
                            }
                        }
                        else
                        {
                            sb.AppendLine($"+ {result.Type}");
                        }
                        
                        sb.AppendLine(result.Message);
                        sb.AppendLine();
                    }
                    
                    sb.AppendLine("```");
                    builder.WithDescription(sb.ToString());
                    
                    await ReplyAsync(embed: builder.Build());
                    return;
                }
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[5];

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[_random.Next(chars.Length)];
            }

            var token = new string(stringChars);

            builder
                .WithColor(Constants.DefaultColour)
                .WithDescription($"{_loading} Please set your Location field to the provided Token below.")
                .WithFooter($"You have {_totalTime.TotalSeconds} seconds. React with ❌ anytime and wait a few seconds to cancel.")
                .AddField("Token", token, true)
                .AddField("Edit Profile", "https://myanimelist.net/editprofile.php", true);
            
            var message = await ReplyAsync(embed: builder.Build());
            
            var success = false;
            var shouldCancel = false;

            _ = Task.Run(async () =>
            {
                var result = await _interactivityService.NextReactionAsync(x => x.MessageId == message.Id && x.UserId == Context.User.Id);

                if (result.IsSuccess && result.Value.Emote.Name == "❌")
                {
                    shouldCancel = true;
                }
            });
            
            builder.Fields.Clear();
            builder.Footer.Text = null;
            
            for (var i = 0; i < _maxRetries; i++)
            {
                if (shouldCancel)
                {
                    await message.ModifyAsync(msg =>
                    {
                        msg.Embed = builder
                            .WithColor(Color.Red)
                            .WithTitle(_failed)
                            .WithDescription("You have cancelled this operation.")
                            .Build();
                    });
                    
                    return;
                }
                
                profilePage = await _profileDownloader.DownloadProfileDocument(username);

                if (profilePage.GetLocation() == token)
                {
                    success = true;
                    break;
                }

                await Task.Delay(_retryWaitPeriod);
            }

            if (success)
            {
                await _accountService.Update(Context.User.Id, profilePage.GetUserId());

                await message.ModifyAsync(msg =>
                {
                    msg.Embed = builder
                        .WithColor(Color.Green)
                        .WithTitle(_succeeded)
                        .WithDescription("Your account has been successfully linked—welcome to the server!")
                        .Build();
                });
            
                return;
            }
            
            await message.ModifyAsync(msg =>
            {
                msg.Embed = builder
                    .WithColor(Color.Red)
                    .WithTitle(_failed)
                    .WithDescription("Did you set your Location field to the provided Token correctly?")
                    .AddField("Current Location", profilePage.GetLocation() ?? "No Location", true)
                    .AddField("Expected Location", token, true)
                    .Build();
            });
        }
    }
}