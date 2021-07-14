
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using MAL.Models;
using MAL.Pages;

namespace MAL.Parsers
{
    public static partial class ProfileParser
    {
        private static readonly Regex _imageUrl = new($"https://cdn.myanimelist.net/images/userimages/(?<userId>[0-9]+).(png|jp(e)?g|gif)");

        [DocumentProperty]
        public static IEnumerable<IElement> GetClearfixElements(this IHtmlDocument htmlDocument)
        {
            return htmlDocument
                .All
                .Where(x => x.ClassName == "clearfix");
        }
        
        [DocumentProperty]
        public static string GetImageUrl(this ProfilePage profilePage)
        {
            var imageElement = profilePage.HtmlDocument
                .Images
                .FirstOrDefault(x => x.ParentElement.ClassName == "user-image mb8");

            return imageElement?.GetAttribute("data-src");
        }

        [DocumentProperty]
        public static int GetUserId(this ProfilePage profilePage)
        {
            var imageUrl = profilePage.GetImageUrl();

            if (imageUrl is null)
            {
                return -1;
            }

            return int.TryParse(_imageUrl
                .Match(profilePage.GetImageUrl())
                .Groups["userId"]
                .Value, out var id) ? id : 0;
        }

        [DocumentProperty]
        public static bool GetSupporter(this ProfilePage profilePage)
        {
            return profilePage.HtmlDocument
                .All
                .First(x => x.ClassName == "link")
                .Descendents()
                .FirstOrDefault()
                .TextContent == "Supporter";
        }

        
        [DocumentProperty]
        public static TimeSpent GetTimeSpent(this ProfilePage profilePage)
        {
            var days = profilePage.HtmlDocument
                .All
                .Where(x => x.ClassName == "di-tc al pl8 fs12 fw-b")
                .Select(x => x.TextContent)
                .Select(x => x.Replace("Days: ", string.Empty)
                    .Replace("\n", string.Empty)
                    .Trim())
                .Select(double.Parse)
                .ToList();

            return new TimeSpent(AnimeDaysWatched: days[0], MangaDaysRead: days[1]);
        }

        public static (List AnimeList, List MangaList) GetLists(this ProfilePage profilePage)
        { 
            var url = $"https://myanimelist.net/{0}/{profilePage.Username}";
            var (animeScore, mangaScore) = profilePage.GetMeanScores();
            var (animeStatistics, mangaStatistics) = profilePage.GetStats();
            var animeList = new List(string.Format(url, "animelist"), animeScore, animeStatistics);
            var mangaList = new List(string.Format(url, "mangalist"), mangaScore, mangaStatistics);
            return (animeList, mangaList);
        }
        
        [DocumentProperty]
        public static FavouriteScores GetMeanScores(this ProfilePage profilePage)
        {
            var meanScore = profilePage.HtmlDocument
                .All
                .Where(x => x.ClassName == "di-tc ar pr8 fs12 fw-b")
                .Select(x =>
                {
                    return double.Parse(x.Children
                        .FirstOrDefault(y => y.ClassName.Contains("score-label score-"))
                        .TextContent);
                })
                .ToList();

            return new FavouriteScores(AnimeScore: meanScore[0], MangaScore: meanScore[1]);
        }

        [DocumentProperty]
        public static (Statistics AnimeStatistics, Statistics MangaStatistics) GetStats(this ProfilePage profilePage)
        {
            var statistics = profilePage.HtmlDocument
                .All
                .Where(x => x.ClassName is "stats anime" or "stats manga")
                .Select(element =>
                {
                    var scores = element.GetElementsByClassName("di-ib fl-r lh10")
                        .Select(y => y.TextContent)
                        .Select(x => x.Replace(",", string.Empty))
                        .Select(int.Parse)
                        .ToList();

                    return new Statistics(
                        Watchimg: scores[0],
                        Completed: scores[1],
                        OnHold: scores[2],
                        Dropped: scores[3],
                        PlanToWatch: scores[4]);
                }).ToList();

            return (statistics[0], statistics[1]);
        }
        
        [DocumentProperty]
        public static IEnumerable<Favourite> GetFavourites(this ProfilePage profilePage, FavouriteSelector favouriteSelector)
        {
            var selection = favouriteSelector switch
            {
                FavouriteSelector.Anime => "anime",
                FavouriteSelector.Manga => "manga",
                _ => "anime" // Fallback to anime in case something goes wrong
            };

            var favourites = profilePage.HtmlDocument
                .All
                .FirstOrDefault(x => x.ClassName == $"favorites-list {selection}")?
                .GetElementsByClassName("di-tc va-t pl8 data");
            
            return favourites?.Select(x =>
            {
                var anchor = x.Children
                    .OfType<IHtmlAnchorElement>()
                    .FirstOrDefault();

                return new Favourite(anchor.TextContent, anchor.Href);
            });
        }
    }
}