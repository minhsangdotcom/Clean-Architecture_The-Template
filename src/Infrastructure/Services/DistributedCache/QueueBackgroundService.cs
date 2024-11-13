using Application.Common.Interfaces.Services.DistributedCache;
using Application.Common.Interfaces.UnitOfWorks;
using Application.UseCases.Tickets.Carts.Pays;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Infrastructure.Services.DistributedCache;

public class QueueBackgroundService(
    IQueueService queueService,
    IServiceProvider serviceProvider,
    ILogger logger
) : BackgroundService
{
    protected const int MAXIMUM_RETRY = 5;
    private const int INITIAL_RETRY_TIME_IN_SEC = 3;
    private const int MAX_RETRY_INCREMENT_SEC = 1;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            PayCartPayload? request = await queueService.DequeueAsync<PayCartPayload>();

            using IServiceScope scope = serviceProvider.CreateScope();
            ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
            if (request != null)
            {
                await Process(sender.Send(request, stoppingToken));
            }

            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
        }
    }

    private async Task Process<T>(ValueTask<QueueResponse<T>> task)
        where T : class
    {
        QueueResponse<T> queueResponse = new();
        int retry = 0;
        int retryDelay = INITIAL_RETRY_TIME_IN_SEC;

        while (retry < MAXIMUM_RETRY)
        {
            queueResponse = await task;

            if (queueResponse.IsSuccess)
            {
                logger.Information(
                    "excuting request {payloadId} has been success!",
                    queueResponse.PayloadId
                );
                break;
            }

            if (queueResponse.ErrorType == null)
            {
                logger.Warning(
                    "Occuring request {requestId} with Unknown error",
                    queueResponse.PayloadId
                );
                break;
            }

            if (queueResponse.ErrorType == QueueErrorType.Persistent)
            {
                await PushToDeadLetterQueue(queueResponse);
                break;
            }

            retry++;
            queueResponse.RetryCount = retry;
            await Task.Delay(TimeSpan.FromSeconds(retryDelay));
            retryDelay += MAX_RETRY_INCREMENT_SEC;
        }

        if (!queueResponse.IsSuccess && queueResponse.ErrorType == QueueErrorType.Transient)
        {
            //logging into db
            await PushToDeadLetterQueue(queueResponse);
        }
    }

    private async Task PushToDeadLetterQueue<T>(QueueResponse<T> response)
        where T : class
    {
        logger.Information("Pushing request {payloadId} to dead letter queue.", response.PayloadId);
        var deadLetterQueue = new DeadLetterQueue()
        {
            RequestId = response.PayloadId!.Value,
            ErrorDetail = response.Error,
            RetryCount = response.RetryCount,
        };
        using IServiceScope scope = serviceProvider.CreateScope();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        await unitOfWork.Repository<DeadLetterQueue>().AddAsync(deadLetterQueue);
        await unitOfWork.SaveAsync();
    }
}
