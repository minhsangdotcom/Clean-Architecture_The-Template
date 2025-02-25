using Application.Common.Interfaces.Services.DistributedCache;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace Infrastructure.Services.DistributedCache;

public class DeadletterQueueBackgroundService(
    IQueueFactory factory,
    IServiceProvider serviceProvider,
    IOptions<QueueSettings> options
) : BackgroundService
{
    private readonly QueueSettings queueSettings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger>();
        IQueueLogService queueLogService =
            scope.ServiceProvider.GetRequiredService<IQueueLogService>();
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessWithRetryAsync<TRequest, TResponse>(
        ValueTask<QueueResponse<TResponse>> task,
        TRequest request,
        ILogger logger,
        IQueueLogService queueLogService,
        CancellationToken cancellationToken
    )
        where TRequest : class
        where TResponse : class
    {
        QueueResponse<TResponse> queueResponse = new();
        int attempt = 0;
        int maximumRetryAttempt = queueSettings.DeadLetterMaxRetryAttempts;
        double maximumDelay = queueSettings.MaximumDelayInSec;

        while (attempt < maximumRetryAttempt)
        {
            queueResponse = await task;

            // sucess case
            if (queueResponse.IsSuccess)
            {
                logger.Information(
                    "Request {payloadId} has been success!",
                    queueResponse.PayloadId
                );
                break;
            }

            // 500 or 400 error
            if (queueResponse.ErrorType == QueueErrorType.Persistent)
            {
                await queueLogService.CreateAsync(queueResponse, request);
                break;
            }

            // transient error retry but
            if (queueResponse.ErrorType == QueueErrorType.Transient)
            {
                attempt++;
                queueResponse.RetryCount = attempt;

                // Calculate delay time with exponential jitter backoff method
                // 1st -> 2.1s; 2nd -> 4.2; 3rd -> 8.2; 4th -> 16.1
                double backoff = Math.Pow(QueueExtention.INIT_DELAY, attempt); // Exponential backoff (2^attempt)
                double jitter = QueueExtention.GenerateJitter(0, QueueExtention.MAXIMUM_JITTER); // Add jitter
                double delay = Math.Min(backoff + jitter, maximumDelay);

                TimeSpan delayTime = TimeSpan.FromSeconds(delay);
                logger.Warning($"Retry {attempt} in {delayTime.TotalSeconds:F2} seconds...");
                await Task.Delay(delayTime, cancellationToken);
            }
        }

        if (!queueResponse.IsSuccess && queueResponse.ErrorType == QueueErrorType.Transient)
        {
            // if it still fail after many attempts then logging into db
            await queueLogService.CreateAsync(queueResponse, request);
        }
    }
}
