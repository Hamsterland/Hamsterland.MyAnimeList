using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Hamsterland.MyAnimeList.Services.Activity
{
    public interface IActivityService
    {
        Task<ActivityScore> GetActivityScores(string username);
    }

    public class ActivityService : IActivityService
    {
        private readonly HttpClient _httpClient;

        public ActivityService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        private const string _rss = "rss.php?type=r{0}&u={1}";
        private const string _anime = "w";
        private const string _manga = "m";
        private const string _pubDate = "pubDate";

        public async Task<ActivityScore> GetActivityScores(string username)
        {
            var animeUrl = $"https://myanimelist.net/{string.Format(_rss, _anime, username)}";
            var mangaUrl = $"https://myanimelist.net/{string.Format(_rss, _manga, username)}";
            var animeScore = await GetRawScore(animeUrl);
            var mangaScore = await GetRawScore(mangaUrl);
            return new ActivityScore(animeScore, mangaScore);
        }
        
        private async Task<double?> GetRawScore(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            
            if (content.StartsWith("fail"))
            {
                return null;
            }
            
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(content);
            
            var dates = new List<DateTime>();
            var reader = new XmlNodeReader(xmlDoc);
            
            while (reader.Read())
            {
                HandlePubDateNode(reader, dates);
            }

            dates = dates
                .OrderBy(x => x)
                .ToList();
            
            var diff = dates
                .Max()
                .Subtract(dates.Min());
            
            var average = TimeSpan.FromTicks(diff.Ticks / dates.Count);
            return average.TotalDays;
        }

        private static void HandlePubDateNode(XmlNodeReader reader, List<DateTime> dates)
        {
            if (reader.Name != _pubDate)
            {
                return;
            }
            
            reader.Read();

            if (string.IsNullOrWhiteSpace(reader.Value))
            {
                return;
            }

            if (DateTime.TryParse(reader.Value.Trim(), out var date))
            {
                dates.Add(date);
            }
        }
    }
}