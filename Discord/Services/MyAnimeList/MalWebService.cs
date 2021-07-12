using System.Net.Http;
using System.Threading.Tasks;

namespace Hamsterland.MyAnimeList.Services.MyAnimeList
{
    public interface IMalWebService
    {
        Task<bool> DoesAccountExist(string username);
    }

    public class MalWebService : IMalWebService
    {
        private readonly HttpClient _httpClient;

        public MalWebService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> DoesAccountExist(string username)
        {
            var url = $"https://myanimelist.net/profile/{username}";
            var response = await _httpClient.GetAsync(url);
            return response.IsSuccessStatusCode;
        }
    }
}