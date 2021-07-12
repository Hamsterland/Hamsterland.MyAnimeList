using Discord.WebSocket;
using MediatR;

namespace Hamsterland.MyAnimeList.Notifications
{
    
    public class UserLeftNotification : INotification
    {
        public SocketGuildUser User { get; }
        
        public UserLeftNotification(SocketGuildUser user)
        {
            User = user;
        }
    }
}