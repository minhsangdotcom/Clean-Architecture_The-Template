using Application.Common.Interfaces.Services;
using Mediator;
using Serilog;

namespace Application.Common.Behaviors;

public class LoggingBehavior<TMessage, TResponse>(
    ILogger logger,
    ICurrentUser currentUser
) : MessagePreProcessor<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    protected override ValueTask Handle(TMessage message, CancellationToken cancellationToken)
    {
        string requestName = typeof(TMessage).Name;
        Ulid? id = currentUser.Id;

        logger.Information(
            "\n\n Application logging incomming request: {Name} has userId {userId} with payload \n {@Request} \n",
            requestName,
            id,
            message
        );

        return default!;
    }
}
