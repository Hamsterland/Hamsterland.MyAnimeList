using System;
using Discord;
using Hamsterland.MyAnimeList;
using Humanizer;
using MAL.Factories;
using MAL.Models;


namespace Uchuumaru.Services.MyAnimeList
{
    public class ProfileEmbedFactory : EmbedBuilder
    {
        public Profile Profile { get; }
        
        public ProfileEmbedFactory(Profile profile)
        {
            Profile = profile;
        }
        
        private readonly Emoji _alarmclock = new("\u23F0");
        private readonly Emoji _maleSign = new("♂️");
        private readonly Emoji _femaleSign = new("♀️");
        private readonly Emoji _nonBinary = new(":zero:");
        private readonly Emoji _unspecified = new("\u2753");
        private readonly Emoji _date = new("\uD83D\uDCC5");
        private readonly Emoji _map = new("🗺️");
        private readonly Emoji _hourGlass = new("\u23F3");
        private readonly Emoji _barChart = new("\uD83D\uDCCA");


        public ProfileEmbedFactory WithDefaultColour()
        {
            WithColor(Constants.DefaultColour);
            return this;
        }
        
        public ProfileEmbedFactory WithName()
        {
            WithTitle($"{Profile.Username}'s Profile");
            WithUrl(Profile.Url);
            return this;
        }

        public ProfileEmbedFactory WithProfileImage()
        {
            if (Profile.ImageUrl is not null)
            {
                WithThumbnailUrl(Profile.ImageUrl);
            }
            
            return this;
        }

        public ProfileEmbedFactory WithListUrls()
        {
            WithDescription($"[Anime List]({Profile.AnimeList.Url}) • [Manga List]({Profile.MangaList.Url})");
            return this;
        }

        public ProfileEmbedFactory WithLastOnline(bool inline = true)
        {
            AddField($"{_alarmclock} Last Online", Profile.LastOnline, inline);
            return this;
        }

        public ProfileEmbedFactory WithAnimeTitle()
        {
            WithTitle($"{Profile.Username}'s Anime Stats");
            WithUrl(Profile.AnimeList.Url);
            return this;
        }

        public ProfileEmbedFactory WithMangaTitle()
        {
            WithTitle($"{Profile.Username}'s Manga Stats");
            WithUrl(Profile.MangaList.Url);
            return this;
        }
        
        public ProfileEmbedFactory WithGender(bool inline = true)
        {
#pragma warning disable 8509
            var emote = Profile.Gender switch
#pragma warning restore 8509
            {
                Gender.Male => _maleSign,
                Gender.Female => _femaleSign,
                Gender.NonBinary => _nonBinary,
                Gender.Unspecified => _unspecified
            };

            AddField($"{emote} Gender", Profile.Gender, inline);
            return this;
        }

        public ProfileEmbedFactory WithBirthday(string backupText = "No Birthday", bool inline = true)
        {
            AddField($"{_date} Birthday", Profile.Birthday ?? backupText, inline);
            return this;
        }

        public ProfileEmbedFactory WithLocation(string backupText = "No Location", bool inline = true)
        {
            AddField($"{_map} Location", Profile.Location ?? backupText, inline);
            return this;
        }

        public ProfileEmbedFactory WithDateJoined(bool inline = true)
        {
            var unixTimestamp = ((DateTimeOffset) Profile.DateJoined).ToUnixTimeSeconds();
            AddField($"{_hourGlass} Joined", $"<t:{unixTimestamp}:D>", inline);
            return this;
        }

        public ProfileEmbedFactory WithMeanScore(bool inline = true)
        {
            AddField($"{_barChart} Mean Score", Profile.AnimeList.Score, inline);
            return this;
        }
        
        public Embed BuildFullEmbed()
        {
            var builder = new ProfileEmbedFactory(Profile)
                .WithName()
                .WithProfileImage()
                .WithListUrls()
                .WithLastOnline()
                .WithGender()
                .WithBirthday()
                .WithLocation()
                .WithDateJoined()
                .WithMeanScore();
            
            return builder.Build();
        }
    }
}