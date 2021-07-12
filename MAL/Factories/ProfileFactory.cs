using System.Linq;
using MAL.Models;
using MAL.Pages;
using MAL.Parsers;

namespace MAL.Factories
{
    public class ProfileFactory
    {
        private readonly ProfilePage _profilePage;
        private readonly Profile _profile = new();

        public ProfileFactory(ProfilePage profilePage)
        {
            _profilePage = profilePage;
            _profile.Username = _profilePage.Username;
            _profile.Id = _profilePage.GetUserId();
            _profile.ImageUrl = _profilePage.GetImageUrl();
        }

        public ProfileFactory WithLocation()
        {
            _profile.Location = _profilePage.GetLocation();
            return this;
        }
        
        public ProfileFactory WithDateJoined()
        {
            _profile.DateJoined = _profilePage.GetDateJoined();
            return this;
        }
        
        public ProfileFactory WithGender()
        {
            _profile.Gender = _profilePage.GetGender();
            return this;
        }
        
        public ProfileFactory WithBirthday()
        {
            _profile.Birthday = _profilePage.GetBirthday();
            return this;
        }

        public ProfileFactory WithLists()
        {
            var (animeList, mangaList) = _profilePage.GetLists();
            _profile.AnimeList = animeList;
            _profile.MangaList = mangaList;
            return this;
        }

        public ProfileFactory WithSupporter()
        {
            _profile.IsSupporter = _profilePage.GetSupporter();
            return this;
        }

        public ProfileFactory WithLastOnline()
        {
            _profile.LastOnline = _profilePage.GetLastOnline();
            return this;
        }

        public ProfileFactory WithFavourites()
        {
            var animeFavourites = _profilePage.GetFavourites(FavouriteSelector.Anime)?.ToList();
            var mangaFavourites = _profilePage.GetFavourites(FavouriteSelector.Manga)?.ToList();
            _profile.AnimeFavourites = animeFavourites;
            _profile.MangaFavourites = mangaFavourites;
            return this;
        }

        public Profile BuildFullProfile()
        {
            WithLocation();
            WithBirthday();
            WithDateJoined();
            WithGender();
            WithLists();
            WithSupporter();
            WithLastOnline();
            WithFavourites();
            return Build();
        }
        
        public Profile Build()
        {
            return _profile;
        }
    }
}