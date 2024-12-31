using Application.Common.Interfaces.UnitOfWorks;
using AutoMapper;
using Domain.Aggregates.Carts;
using Mediator;

namespace Application.Features.Tickets.Carts.Create;

public class CreateCartHandler(IUnitOfWork unitOfWork, IMapper mapper)
    : IRequestHandler<CreateCartCommand, CreateCartResponse>
{
    public async ValueTask<CreateCartResponse> Handle(
        CreateCartCommand command,
        CancellationToken cancellationToken
    )
    {
        var mappingCart = mapper.Map<Cart>(command);

        Cart cart = await unitOfWork.Repository<Cart>().AddAsync(mappingCart, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);

        return mapper.Map<CreateCartResponse>(cart);
    }
}
