using System.Diagnostics;
using Mediator;
using Serilog;

namespace Application.Common.Behaviors;

public class PerformaceBehavior<TMessage, TResponse>(ILogger logger)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : notnull, IMessage
{
    private readonly Stopwatch timer = new();

    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken
    )
    {
        timer.Start();

        TResponse response = await next(message, cancellationToken);

        timer.Stop();

        long elapsedMilliseconds = timer.ElapsedMilliseconds;

        string requestName = typeof(TMessage).Name;

        logger.Information(
            "\n\nProcessing {Name} request in ({ElapsedMilliseconds} milliseconds)\n\n",
            requestName,
            elapsedMilliseconds
        );

        return response;
    }
}
