using Mediator;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public sealed class ErrorLoggingBehaviour<TMessage, TResponse>(
    ILogger<ErrorLoggingBehaviour<TMessage, TResponse>> logger
) : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        CancellationToken cancellationToken,
        MessageHandlerDelegate<TMessage, TResponse> next
    )
    {
        try
        {
            return await next(message, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(
                "\n\n Server {exception} error is with message '{Message}'\n {StackTrace}\n at {DatetimeUTC} \n",
                exception.GetType().Name,
                exception.Message,
                exception.StackTrace?.TrimStart(),
                DateTimeOffset.UtcNow
            );
            throw;
        }
    }
}
