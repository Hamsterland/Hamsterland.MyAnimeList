using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace MAL.Downloaders
{
    public class DownloaderBase
    {
        private protected HttpClient _httpClient = new();
        private protected HtmlParser _htmlParser = new();

        private protected virtual async Task<IHtmlDocument> DownloadHtmlDocument(string url)
        {
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var html = await response.Content.ReadAsStringAsync();
            return await _htmlParser.ParseDocumentAsync(html, CancellationToken.None);
        }
    }
}