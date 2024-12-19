using Application.Common.Interfaces.Services.DistributedCache;
using Application.Common.Interfaces.UnitOfWorks;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infrastructure.Services.DistributedCache;

public class QueueBackgroundService(IServiceProvider serviceProvider) : BackgroundService
{
    protected const int MAXIMUM_RETRY = 10;
    private const int INITIAL_RETRY_TIME_IN_SEC = 3;
    private const int MAX_RETRY_INCREMENT_SEC = 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using IServiceScope scope = serviceProvider.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILogger>();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        while (!stoppingToken.IsCancellationRequested)
        {
            // PayCartPayload? request = await queueService.DequeueAsync<PayCartPayload>();
            // if (request != null)
            // {
            //     await Process(sender.Send(request, stoppingToken), logger, unitOfWork);
            // }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private static async Task Process<T>(
        ValueTask<QueueResponse<T>> task,
        ILogger logger,
        IUnitOfWork unitOfWork
    )
        where T : class
    {
        QueueResponse<T> queueResponse = new();
        int retry = 0;
        int retryDelay = INITIAL_RETRY_TIME_IN_SEC;

        while (retry < MAXIMUM_RETRY)
        {
            queueResponse = await task;

            // sucess case
            if (queueResponse.IsSuccess)
            {
                logger.Information(
                    "excuting request {payloadId} has been success!",
                    queueResponse.PayloadId
                );
                break;
            }

            // 500 or 400 error
            if (queueResponse.ErrorType == QueueErrorType.Persistent)
            {
                await PushToDeadLetterQueue(queueResponse, logger, unitOfWork);
                break;
            }

            // transient error retry but
            if (queueResponse.ErrorType == QueueErrorType.Transient)
            {
                retry++;
                queueResponse.RetryCount = retry;
                await Task.Delay(TimeSpan.FromSeconds(retryDelay));
                retryDelay += MAX_RETRY_INCREMENT_SEC;
            }
        }

        if (!queueResponse.IsSuccess && queueResponse.ErrorType == QueueErrorType.Transient)
        {
            //logging into db
            await PushToDeadLetterQueue(queueResponse, logger, unitOfWork);
        }
    }

    private static async Task PushToDeadLetterQueue<T>(
        QueueResponse<T> response,
        ILogger logger,
        IUnitOfWork unitOfWork
    )
        where T : class
    {
        logger.Information("Pushing request {payloadId} to dead letter queue.", response.PayloadId);
        var deadLetterQueue = new DeadLetterQueue()
        {
            RequestId = response.PayloadId!.Value,
            ErrorDetail = response.Error,
            RetryCount = response.RetryCount,
        };
        await unitOfWork.Repository<DeadLetterQueue>().AddAsync(deadLetterQueue);
        await unitOfWork.SaveAsync();
    }
}
