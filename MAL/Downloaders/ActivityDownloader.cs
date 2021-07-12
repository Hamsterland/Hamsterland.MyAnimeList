using System;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using MAL.Pages;

namespace MAL.Downloaders
{
    public class ActivityDownloader : DownloaderBase 
    {
        private const string _rss = "rss.php?type=r{0}&u={1}";
        private const string _anime = "w";
        private const string _manga = "m";
        
        public async Task<ActivityPage> GetRssFeeds(string username)
        {
            var animeUrl = "https://myanimelist.net/" + string.Format(_rss, _anime, username);
            var mangaUrl = "https://myanimelist.net/" + string.Format(_rss, _manga, username);
            var animeResponse = await _httpClient.GetAsync(animeUrl);
            var mangaResponse = await _httpClient.GetAsync(mangaUrl);

            string animeRss = null;
            string mangaRss = null;

            if (animeResponse.IsSuccessStatusCode)
            {
                animeRss = await animeResponse.Content.ReadAsStringAsync();
            }

            if (mangaResponse.IsSuccessStatusCode)
            {
                mangaRss = await mangaResponse.Content.ReadAsStringAsync();
            }

            return new ActivityPage
            {
                AnimeRss = animeRss,
                MangaRss = mangaRss
            };
        }
        
        private protected override Task<IHtmlDocument> DownloadHtmlDocument(string url)
        {
            throw new NotImplementedException();
        }
    }
}