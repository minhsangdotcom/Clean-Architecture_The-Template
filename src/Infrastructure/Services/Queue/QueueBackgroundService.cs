using Application.Common.Interfaces.Services.Queue;
using Application.Features.QueueLogs;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.Queue;

public class QueueBackgroundService(
    IQueueService queueService,
    IServiceProvider serviceProvider,
    IOptions<QueueSettings> options
) : BackgroundService
{
    private readonly QueueSettings queueSettings = options.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<QueueBackgroundService>>();

        while (!stoppingToken.IsCancellationRequested)
        {
            // PayCartPayload? request = await queueService.DequeueAsync<
            //     PayCartPayload,
            //     PayCartRequest
            // >();

            if (!await queueService.PingAsync())
            {
                logger.LogWarning("Redis server is unavailable!");
                continue;
            }

            // if (request != null)
            // {
            //     await ProcessWithRetryAsync<PayCartPayload, PayCartResponse>(
            //         request,
            //         sender,
            //         logger,
            //         stoppingToken
            //     );
            // }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task ProcessWithRetryAsync<TRequest, TResponse>(
        TRequest request,
        ISender sender,
        ILogger<QueueBackgroundService> logger,
        CancellationToken cancellationToken
    )
        where TRequest : class
        where TResponse : class
    {
        QueueResponse<TResponse>? queueResponse = new();
        int attempt = 0;
        int maximumRetryAttempt = queueSettings.MaxRetryAttempts;
        double maximumDelay = queueSettings.MaximumDelayInSec;

        while (attempt <= maximumRetryAttempt)
        {
            queueResponse =
                await sender.Send(request, cancellationToken) as QueueResponse<TResponse>;

            // success case
            if (queueResponse!.IsSuccess)
            {
                logger.LogInformation(
                    "Executing request {payloadId} has been success!",
                    queueResponse.PayloadId
                );
                break;
            }

            // 500 or 400 error
            if (queueResponse.ErrorType == QueueErrorType.Persistent)
            {
                CreateQueueLogCommand createQueueLogCommand =
                    new()
                    {
                        RequestId = queueResponse.PayloadId!.Value,
                        ErrorDetail = queueResponse.Error,
                        Request = request,
                        RetryCount = attempt,
                    };
                await sender.Send(createQueueLogCommand, cancellationToken);
                break;
            }

            // transient error retry
            attempt++;
            if (attempt > maximumRetryAttempt)
            {
                break;
            }

            queueResponse.RetryCount = attempt;

            // Calculate delay time with exponential jitter backoff method
            // 1st -> 2.1s; 2nd -> 4.2; 3rd -> 8.2; 4th -> 16.1
            double backoff = Math.Pow(QueueExtension.INIT_DELAY, attempt); // Exponential backoff (2^attempt)
            double jitter = QueueExtension.GenerateJitter(0, QueueExtension.MAXIMUM_JITTER); // Add jitter
            double delay = Math.Min(backoff + jitter, maximumDelay);

            TimeSpan delayTime = TimeSpan.FromSeconds(delay);
            logger.LogWarning(
                "Retry {Attempt} in {DelayTimeTotalSeconds:F2} seconds...",
                attempt,
                delayTime.TotalSeconds
            );
            await Task.Delay(delayTime, cancellationToken);
        }

        if (queueResponse is { IsSuccess: false, ErrorType: QueueErrorType.Transient })
        {
            // if it still fails after many attempts then logging into db
            await sender.Send(
                new CreateQueueLogCommand()
                {
                    RequestId = queueResponse.PayloadId!.Value,
                    ErrorDetail = queueResponse.Error,
                    Request = request,
                    RetryCount = queueResponse.RetryCount,
                },
                cancellationToken
            );
        }
    }
}
