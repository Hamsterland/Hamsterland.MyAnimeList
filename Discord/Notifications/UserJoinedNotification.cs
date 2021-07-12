using Discord.WebSocket;
using MediatR;

namespace Hamsterland.MyAnimeList.Notifications
{
    public class UserJoinedNotification : INotification
    {
        public SocketGuildUser User { get; }
        
        public UserJoinedNotification(SocketGuildUser user)
        {
            User = user;
        }
    }
}