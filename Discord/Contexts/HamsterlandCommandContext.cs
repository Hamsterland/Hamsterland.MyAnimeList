using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace Hamsterland.MyAnimeList.Contexts
{
    public class HamsterlandCommandContext : SocketCommandContext
    {
        public IServiceScope ServiceScope { get; } 
        
        public HamsterlandCommandContext(DiscordSocketClient client, SocketUserMessage msg, IServiceScope serviceScope) : base(client, msg)
        {
            ServiceScope = serviceScope;
        }
    }
}