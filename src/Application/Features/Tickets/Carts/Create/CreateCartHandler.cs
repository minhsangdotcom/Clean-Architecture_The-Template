using Application.Common.Interfaces.UnitOfWorks;
using Contracts.ApiWrapper;
using Domain.Aggregates.Carts;
using Domain.Aggregates.Carts.Specifications;
using Mediator;

namespace Application.Features.Tickets.Carts.Create;

public class CreateCartHandler(IUnitOfWork unitOfWork)
    : IRequestHandler<CreateCartCommand, Result<CreateCartResponse>>
{
    public async ValueTask<Result<CreateCartResponse>> Handle(
        CreateCartCommand command,
        CancellationToken cancellationToken
    )
    {
        var mappingCart = command.ToCart();

        Cart createdCart = await unitOfWork
            .Repository<Cart>()
            .AddAsync(mappingCart, cancellationToken);
        await unitOfWork.SaveAsync(cancellationToken);
        Cart? cart = await unitOfWork
            .DynamicReadOnlyRepository<Cart>()
            .FindByConditionAsync(
                new FindCartByIdIncludeSpecification(createdCart.Id),
                cancellationToken
            );
        return Result<CreateCartResponse>.Success(cart!.ToCreateCartResponse());
    }
}
