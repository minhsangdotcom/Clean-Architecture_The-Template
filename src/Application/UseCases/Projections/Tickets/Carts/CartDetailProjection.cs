using Application.UseCases.Projections.Customers;
using Domain.Aggregates.Carts.Enums;

namespace Application.UseCases.Projections.Tickets.Carts;

public class CartDetailProjection
{
    public string? ShippingAddress { get; set; }

    public int ShippingFee { get; set; }

    public string? PhoneNumber { get; set; }

    public int Total { get; set; }

    public CartStatus CartStatus { get; set; }

    public string? InvalidReason { get; set; }

    public CustomerProjection? Customer { get; set; }

    public IEnumerable<CartItemProjection>? CartItems { get; set; }
}
