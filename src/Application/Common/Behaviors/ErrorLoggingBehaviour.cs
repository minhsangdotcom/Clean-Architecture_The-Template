using Mediator;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public sealed class ErrorLoggingBehaviour<TMessage, TResponse>(
    ILogger<ErrorLoggingBehaviour<TMessage, TResponse>> logger
) : MessageExceptionHandler<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    protected override ValueTask<ExceptionHandlingResult<TResponse>> Handle(
        TMessage message,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(
            "Server {exception} error is with message '{Message}'\n {StackTrace}\n at {DatetimeUTC}",
            exception.GetType().Name,
            exception.Message,
            exception.StackTrace?.TrimStart(),
            DateTimeOffset.UtcNow
        );

        return NotHandled;
    }
}
