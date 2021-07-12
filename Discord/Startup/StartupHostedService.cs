using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Hamsterland.MyAnimeList.Startup
{
    public class StartupHostedService : IHostedService
    {
        private readonly DiscordSocketClient _client;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        
        public StartupHostedService(
            DiscordSocketClient client, 
            IConfiguration configuration,
            ILogger logger)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var token = _configuration["Discord:Token"];
            
            if (string.IsNullOrEmpty(token))
            {
                _logger.Fatal("The bot token was not found in appsettings.json");
                return; 
            }
            
            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _client.LogoutAsync();
            await _client.StopAsync();
        }
    }
}