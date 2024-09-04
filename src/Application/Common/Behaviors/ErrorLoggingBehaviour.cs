using Mediator;
using Microsoft.Extensions.Logging;
namespace Application.Common.Behaviors;
public sealed class ErrorLoggingBehaviour<TMessage, TResponse>(ILogger<ErrorLoggingBehaviour<TMessage, TResponse>> logger) : MessageExceptionHandler<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    protected override ValueTask<ExceptionHandlingResult<TResponse>> Handle(
        TMessage message,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "Error handling message of type {messageType}", message.GetType().Name);
        return NotHandled;
    }
}
