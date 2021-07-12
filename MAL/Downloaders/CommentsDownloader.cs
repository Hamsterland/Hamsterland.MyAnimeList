using System.Threading.Tasks;
using MAL.Pages;

namespace MAL.Downloaders
{
    public class CommentsDownloader : DownloaderBase
    {
        public async Task<CommentsPage> DownloadCommentsDocument(int id)
        {
            var htmlDocument = await DownloadHtmlDocument($"https://myanimelist.net/comments.php?id={id}");
            
            return new CommentsPage
            {
                HtmlDocument = htmlDocument
            };
        }
    }
}