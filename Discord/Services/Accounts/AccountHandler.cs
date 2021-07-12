using System.Threading;
using System.Threading.Tasks;
using Hamsterland.MyAnimeList.Notifications;
using MediatR;

namespace Hamsterland.MyAnimeList.Services.Accounts
{
    public class AccountHandler 
        : INotificationHandler<MessageReceivedNotification>, 
            INotificationHandler<UserJoinedNotification>,
            INotificationHandler<UserLeftNotification>
    {
        private readonly IAccountService _accountService;

        public AccountHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
        {
            await HandleAccountCreation(notification.Message.Author.Id);
        }

        public async Task Handle(UserJoinedNotification notification, CancellationToken cancellationToken)
        {
            await HandleAccountCreation(notification.User.Id);
        }

        public async Task Handle(UserLeftNotification notification, CancellationToken cancellationToken)
        {
            await HandleAccountDeletion(notification.User.Id);
        }

        private async Task HandleAccountCreation(ulong userId)
        {
            var accountDto = await _accountService.GetByUserId(userId);

            if (accountDto is not null)
            {
                return;
            }

            await _accountService.Create(userId);
        }

        private async Task HandleAccountDeletion(ulong userId)
        {
            var accountDto = await _accountService.GetByUserId(userId);

            if (accountDto is null)
            {
                return;
            }

            await _accountService.Delete(userId);
        }
    }
}