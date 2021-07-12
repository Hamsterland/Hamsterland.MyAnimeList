using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MAL.Pages;
using MAL.Parsers;

namespace MAL.Downloaders
{
    public class UsersDownloader : DownloaderBase
    {
        public async Task<UsersPage> DownloadUsersDocument(string username)
        {
            var htmlDocument = await DownloadHtmlDocument($"https://myanimelist.net/users.php?q={username}&cat=user");

            var similarUsernames = new List<string>();
            
            if (htmlDocument is not null)
            {
                similarUsernames = htmlDocument
                    .GetSimilarUsernames()
                    .Skip(1)
                    .ToList();
            
                similarUsernames.RemoveAt(similarUsernames.Count - 1);
            }
            
            return new UsersPage
            {
                Username = username,
                HtmlDocument = htmlDocument,
                SimilarUsernames = similarUsernames.ToArray()
            };
        }
    }
}