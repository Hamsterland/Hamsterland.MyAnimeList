using Discord;
using MediatR;

namespace Hamsterland.MyAnimeList.Notifications
{
    public class LogMessageNotification : INotification
    {
        public LogMessage LogMessage { get; }

        public LogMessageNotification(LogMessage logMessage)
        {
            LogMessage = logMessage;
        }
    }
}