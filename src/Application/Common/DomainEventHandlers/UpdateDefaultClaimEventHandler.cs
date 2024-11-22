using Application.Common.Interfaces.Services.Identity;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Events;
using Mediator;

namespace Application.Common.DomainEventHandlers;

public class UpdateDefaultClaimEventHandler(IUserManagerService userManagerService) : INotificationHandler<UpdateDefaultUserClaimEvent>
{
    public async ValueTask Handle(
        UpdateDefaultUserClaimEvent notification,
        CancellationToken cancellationToken
    )
    {
        User user = notification.User!;
        IEnumerable<UserClaim> defaultUserClaims = user.DefaultUserClaimsToUpdates;
        await userManagerService.ReplaceDefaultClaimsToUserAsync(defaultUserClaims);
    }
}
