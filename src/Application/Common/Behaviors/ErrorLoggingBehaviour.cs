using System.Diagnostics;
using Contracts.ApiWrapper;
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
            "\n\n Server {exception} error has {@trace}  error is with message '{Message}'\n {StackTrace}\n at {DatetimeUTC} \n",
            exception.GetType().Name,
            new TraceLogging()
            {
                TraceId = Activity.Current?.TraceId.ToString(),
                SpanId = Activity.Current?.SpanId.ToString()
            },
            exception.Message,
            exception.StackTrace?.TrimStart(),
            DateTimeOffset.UtcNow
        );

        return NotHandled;
    }
}
