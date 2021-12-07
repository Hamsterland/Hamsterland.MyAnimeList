using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hamsterland.MyAnimeList.Services.Activity;
using MAL.Downloaders;
using MAL.Factories;
using MAL.Models;

namespace Hamsterland.MyAnimeList.Services.Verification
{
    public class Verifier
    {
        private readonly ProfileDownloader _profileDownloader = new();
        private readonly List<VerifierResult> _results = new();
        private readonly TimeSpan _minimumAccountAge = TimeSpan.FromDays(30);

        public async Task<List<VerifierResult>> Verify(string username, IActivityService activityService)
        {
            var profilePage = await _profileDownloader.DownloadProfileDocument(username);
            
            var profile = new ProfileFactory(profilePage)
                .WithDateJoined()
                .WithLists()
                .Build();

            CheckProfileImage(profile);
            CheckAccountAge(profile);

            // ReSharper disable once UseDeconstruction
            var activityScore = await activityService.GetActivityScores(username);
            CheckListPublicity(activityScore);
            CheckAccountActivity(activityScore);

            return _results;
        }

        private void CheckAccountActivity(ActivityScore activityScore)
        {
            if (MeetsMinimumActivityThreshold(activityScore))
            {
                _results.Add(VerifierResult.FromSuccess(VerifierResultType.Activity,"Your account meets the list activity requirements."));
            }
            else
            {
                _results.Add(VerifierResult.FromError(VerifierResultType.Activity,
                    "Your account is not active enough. Show regular activity by updating your list. Spamming list entries will not help. Use MAL regularly and try again later"));
            }
        }

        private void CheckListPublicity(ActivityScore activityScore)
        {
            const string privateListMessage = "Your {0} list is private. Please set it to public (you can revert this after verification).";
            const string publicListMessage = "Your {0} list is public and accessible.";

            _results.Add(AnimeListIsPublic(activityScore)
                ? VerifierResult.FromSuccess(VerifierResultType.Lists,string.Format(publicListMessage, "anime"))
                : VerifierResult.FromError(VerifierResultType.Lists, string.Format(privateListMessage, "anime")));

            _results.Add(MangaListIsPublic(activityScore)
                ? VerifierResult.FromSuccess(VerifierResultType.Lists,string.Format(publicListMessage, "manga"))
                : VerifierResult.FromError(VerifierResultType.Lists, string.Format(privateListMessage, "manga")));
        }

        private void CheckAccountAge(Profile profile)
        {
            if (MeetsMinimumAccountAge(profile))
            {
                _results.Add(VerifierResult.FromSuccess(VerifierResultType.Age, "Your account is more than 30 days old."));
            }
            else
            {
                var remaining = _minimumAccountAge - (DateTime.Now - profile.DateJoined);
                _results.Add(VerifierResult.FromError(VerifierResultType.Age, $"Your account needs to be at least 30 days old. Please wait {remaining.Days} more days."));
            }
        }

        private void CheckProfileImage(Profile profile)
        {
            _results.Add(HasProfileImage(profile)
                ? VerifierResult.FromSuccess(VerifierResultType.Image, $"Your account contains a picture.")
                : VerifierResult.FromError(VerifierResultType.Image, "Your account must contain a picture.", "https://i.imgur.com/tU3Vr8i.png"));
        }

        private static bool HasProfileImage(Profile profile)
        {
            return profile.ImageUrl is not null;
        }

        private bool MeetsMinimumAccountAge(Profile profile)
        {
            return profile.AccountAge.Ticks > _minimumAccountAge.Ticks;
        }

        private static bool AnimeListIsPublic(ActivityScore activityScore)
        {
            return activityScore.Anime.HasValue;
        }
        
        private static bool MangaListIsPublic(ActivityScore activityScore)
        {
            return activityScore.Manga.HasValue;
        }
        
        // ReSharper disable once UseDeconstructionOnParameter
        private static bool MeetsMinimumActivityThreshold(ActivityScore activityScore)
        {
            // return (1 / (activityScore?.Anime + 7.5)) + (1 / (activityScore?.Manga + 7.5)) > 1 / 7.5;
            
            if (activityScore.Anime is > 0.1 and < 14)
            {
                return true;
            }
            
            // ReSharper disable once ConvertIfStatementToReturnStatement
            if (activityScore.Manga is > 0.1 and < 14)
            {
                return true;
            }
                
            return false;
        }
    }
}