using Domain.Aggregates.Carts.Enums;
using Domain.Specs;
using MassTransit.NewIdFormatters;

namespace Domain.Aggregates.Carts.Specifications;

public class FindCartByIdIncludeSpecification : Specification<Cart>
{
    public FindCartByIdIncludeSpecification(Ulid id)
    {
        Query
            .Where(x => x.Id == id && !x.IsPaid && x.CartStatus == CartStatus.Pending)
            .Include(x => x.Customer)
            .Include(x => x.CartItems)!
            .ThenInclude(x => x.Ticket)
            .AsSplitQuery();
    }
}
