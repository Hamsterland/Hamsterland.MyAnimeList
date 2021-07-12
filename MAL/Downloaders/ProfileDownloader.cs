using System.Linq;
using System.Threading.Tasks;
using MAL.Pages;
using MAL.Parsers;

namespace MAL.Downloaders
{
    public class ProfileDownloader : DownloaderBase
    {
        public async Task<ProfilePage> DownloadProfileDocument(string username)
        {
            var htmlDocument = await DownloadHtmlDocument($"https://myanimelist.net/profile/{username}");

            return new ProfilePage
            {
                Username = username,
                HtmlDocument = htmlDocument,
                ClearfixElements = htmlDocument
                    .GetClearfixElements()
                    .ToList()
            };
        }
    }
}