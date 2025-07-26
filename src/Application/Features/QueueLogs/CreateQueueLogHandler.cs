using Application.Common.Interfaces.UnitOfWorks;
using Domain.Aggregates.QueueLogs;
using Mediator;
using Microsoft.Extensions.Logging;

namespace Application.Features.QueueLogs;

public class CreateQueueLogHandler(IUnitOfWork unitOfWork, ILogger<CreateQueueLogHandler> logger)
    : IRequestHandler<CreateQueueLogCommand>
{
    public async ValueTask<Unit> Handle(
        CreateQueueLogCommand command,
        CancellationToken cancellationToken
    )
    {
        logger.LogInformation("Pushing request {payloadId} to logging queue.", command.RequestId);
        await unitOfWork.Repository<QueueLog>().AddAsync(command.ToQueueLog(), cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        return Unit.Value;
    }
}
