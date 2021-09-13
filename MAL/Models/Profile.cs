using System;
using System.Collections.Generic;

namespace MAL.Models
{
    public class Profile
    {
        public string Username { get; set; }
        public int Id { get; set; }
        public string Url => $"https://myanimelist.net/profile/{Username}";
        public string ImageUrl { get; set; }
        public string Location { get; set; }
        public string Birthday { get; set; }
        public DateTime DateJoined { get; set; }
        public TimeSpan AccountAge => DateTime.Now - DateJoined;
        public Gender Gender { get; set; }
        public List AnimeList { get; set; }
        public List MangaList { get; set; }
        public bool IsSupporter { get; set; }
        public string LastOnline { get; set; }
        public List<Favourite> AnimeFavourites { get; set; }
        public List<Favourite> MangaFavourites { get; set; }
        public List<Favourite> PeopleFavourites { get; set; }
    }
}