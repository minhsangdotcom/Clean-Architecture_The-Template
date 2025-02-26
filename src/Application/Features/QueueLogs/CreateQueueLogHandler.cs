using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper;
using Domain.Aggregates.QueueLogs;
using Mediator;
using Serilog;

namespace Application.Features.QueueLogs;

public class CreateQueueLogHandler(IUnitOfWork unitOfWork, IMapper mapper, ILogger logger)
    : IRequestHandler<CreateQueueLogCommand>
{
    public async ValueTask<Unit> Handle(
        CreateQueueLogCommand command,
        CancellationToken cancellationToken
    )
    {
        logger.Information("Pushing request {payloadId} to logging queue.", command.RequestId);
        await unitOfWork
            .Repository<QueueLog>()
            .AddAsync(mapper.Map<QueueLog>(command), cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        return Unit.Value;
    }
}
