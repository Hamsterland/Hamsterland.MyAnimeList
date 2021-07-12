namespace MAL.Models.Results
{
    public class ActivityResult
    {
        public bool IsAnimeListPrivate => !AnimeActivityScore.HasValue;
        public bool IsMangaListPrivate => !MangaActivityScore.HasValue;
        public double? AnimeActivityScore { get; set; }
        public double? MangaActivityScore { get; set; }
    }
}