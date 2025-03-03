using Domain.Aggregates.Carts.Enums;
using Domain.Common.Specs;

namespace Domain.Aggregates.Carts.Specifications;

public class FindCartByIdIncludeSpecification : Specification<Cart>
{
    public FindCartByIdIncludeSpecification(Ulid id)
    {
        Query
            .Where(x => x.Id == id)
            .Include(x => x.Customer)
            .Include(x => x.CartItems)!
            .ThenInclude(x => x.Ticket)
            .AsSplitQuery();
    }
}
