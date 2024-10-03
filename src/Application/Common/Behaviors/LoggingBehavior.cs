using Application.Common.Interfaces.Services;
using Mediator;
using Serilog;

namespace Application.Common.Behaviors;

public class LoggingBehavior<TMessage, TResponse>(ILogger logger, ICurrentUser currentUser)
    : MessagePreProcessor<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    protected override ValueTask Handle(TMessage message, CancellationToken cancellationToken)
    {
        string requestName = typeof(TMessage).Name;
        Ulid? id = currentUser.Id;

        const string replacePhrase = "is userId {userId}";
        string loggingMessage =
            "\n\n Application logs incomming request: {Name} "
            + replacePhrase
            + " with payload \n {@Request} \n";

        List<object> parameters = [requestName, id, message];

        if (id == null)
        {
            loggingMessage = loggingMessage.Replace(
                $"{replacePhrase}",
                "is anonymous",
                StringComparison.Ordinal
            );
            parameters.RemoveAt(1);
        }

        logger.Information(loggingMessage, [.. parameters]);
        return default!;
    }
}
