using Contracts.Constants;
using Contracts.Extensions;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Common.Behaviors;

public sealed class ErrorLoggingBehaviour<TMessage, TResponse>(
    ILogger<ErrorLoggingBehaviour<TMessage, TResponse>> logger,
    IHttpContextAccessor httpContextAccessor
) : MessageExceptionHandler<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    protected override ValueTask<ExceptionHandlingResult<TResponse>> Handle(
        TMessage message,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        if (httpContextAccessor.HttpContext != null)
        {
            httpContextAccessor.HttpContext.Items[Global.TRACE_ID] =
                StringExtension.GenerateRandomString(12);
        }

        logger.LogError(
            "Server {exception} error has trace id {Id} with message '{Message}'\n {StackTrace}\n at {DatetimeUTC}",
            exception.GetType().Name,
            httpContextAccessor?.HttpContext?.Items[Global.TRACE_ID]?.ToString(),
            exception.Message,
            exception.StackTrace?.TrimStart(),
            DateTimeOffset.UtcNow
        );

        return NotHandled;
    }
}
