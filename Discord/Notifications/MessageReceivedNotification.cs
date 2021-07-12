using Discord.WebSocket;
using MediatR;

namespace Hamsterland.MyAnimeList.Notifications
{
    public class MessageReceivedNotification : INotification
    {
        public SocketMessage Message { get; }
        
        public MessageReceivedNotification(SocketMessage message)
        {
            Message = message;
        }
    }
}