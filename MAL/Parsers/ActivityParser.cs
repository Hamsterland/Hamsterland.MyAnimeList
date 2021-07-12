using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MAL.Models.Results;
using MAL.Pages;

namespace MAL.Parsers
{
    public static class ActivityParser
    {
        public static ActivityResult GetListActivityScores(this ActivityPage activityPage)
        {
            var activityResult = new ActivityResult();

            if (activityPage.AnimeRss is null)
            {
                activityResult.AnimeActivityScore = null;
            }
            else
            {
                activityResult.AnimeActivityScore = GetListActivityScore(activityPage.AnimeRss);
            }
            
            if (activityPage.MangaRss is null)
            {
                activityResult.MangaActivityScore = null;
            }
            else
            {
                activityResult.MangaActivityScore = GetListActivityScore(activityPage.MangaRss);
            }
            
            return activityResult;
        }

        private static double GetListActivityScore(string rss)
        {
            if (rss.StartsWith("fail"))
            {
                return 0;
            }
            
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rss);
            
            var dates = new List<DateTime>();
            var reader = new XmlNodeReader(xmlDoc);
            
            while (reader.Read())
            {
                if (reader.Name != "pubDate")
                {
                    continue;
                }
                
                reader.Read();
                    
                if (string.IsNullOrWhiteSpace(reader.Value))
                {
                    continue;
                }

                if (DateTime.TryParse(reader.Value.Trim(), out var date))
                {
                    dates.Add(date);
                }
            }

            dates = dates.OrderBy(x => x).ToList();
            var diff = dates.Max().Subtract(dates.Min());
            var average = TimeSpan.FromTicks(diff.Ticks / dates.Count);
            
            return average.TotalDays;
        }
    }
}