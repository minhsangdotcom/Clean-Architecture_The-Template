using System.Diagnostics;
using Contracts.ApiWrapper;
using Mediator;
using Serilog;

namespace Application.Common.Behaviors;

public sealed class ErrorLoggingBehaviour<TMessage, TResponse>(ILogger logger)
    : MessageExceptionHandler<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    protected override ValueTask<ExceptionHandlingResult<TResponse>> Handle(
        TMessage message,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.Error(
            "\n\n Server {exception} error has {@trace}  error is with message '{Message}'\n {StackTrace}\n at {DatetimeUTC} \n",
            exception.GetType().Name,
            new TraceLogging()
            {
                TraceId = Activity.Current?.TraceId.ToString(),
                SpanId = Activity.Current?.SpanId.ToString(),
            },
            exception.Message,
            exception.StackTrace?.TrimStart(),
            DateTimeOffset.UtcNow
        );

        return NotHandled;
    }
}
