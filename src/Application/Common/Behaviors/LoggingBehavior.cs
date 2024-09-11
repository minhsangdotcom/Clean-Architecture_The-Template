using Application.Common.Interfaces.Services;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public class LoggingBehavior<TMessage, TResponse>(
    ILogger<TMessage> logger,
    ICurrentUser currentUser
) : MessagePreProcessor<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    protected override ValueTask Handle(TMessage message, CancellationToken cancellationToken)
    {
        var requestName = typeof(TMessage).Name;

        var id = currentUser.Id;

        logger.LogInformation(
            "Application logging incomming request: {Name} with userId {userId} and \n {@Request}",
            requestName,
            id,
            message
        );

        return default!;
    }
}
