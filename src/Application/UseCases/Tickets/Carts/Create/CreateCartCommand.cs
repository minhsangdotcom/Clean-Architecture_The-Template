using Domain.Aggregates.Carts;
using Mediator;

namespace Application.UseCases.Tickets.Carts.Create;

public class CreateCartCommand : IRequest<CreateCartResponse>
{
    // i'm so lazy to build another login logic for customer
    public Ulid CustomerId { get; set; }

    public string? ShippingAddress { get; set; }

    public int ShippingFee { get; set; }

    public string? PhoneNumber { get; set; }

    public int Total { get; set; }

    public List<CartItemCommand>? CartItems { get; set; }
}

public class CartItemCommand
{
    public Ulid TiketId { get; set; }

    public int Quantity { get; set; }

    public int TotalPrice { get; set; }
}
