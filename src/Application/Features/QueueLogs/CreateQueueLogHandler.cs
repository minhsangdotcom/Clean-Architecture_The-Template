using Application.Common.Interfaces.UnitOfWorks;
using Domain.Aggregates.QueueLogs;
using Mediator;
using Serilog;

namespace Application.Features.QueueLogs;

public class CreateQueueLogHandler(IUnitOfWork unitOfWork, ILogger logger)
    : IRequestHandler<CreateQueueLogCommand>
{
    public async ValueTask<Unit> Handle(
        CreateQueueLogCommand command,
        CancellationToken cancellationToken
    )
    {
        logger.Information("Pushing request {payloadId} to logging queue.", command.RequestId);
        await unitOfWork.Repository<QueueLog>().AddAsync(command.ToQueueLog(), cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        return Unit.Value;
    }
}
