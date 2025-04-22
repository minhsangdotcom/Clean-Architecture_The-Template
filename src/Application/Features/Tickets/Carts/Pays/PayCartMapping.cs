using Application.Features.Common.Projections.Tickets;
using Application.Features.Common.Projections.Tickets.Carts;
using Domain.Aggregates.Carts;
using Domain.Aggregates.Orders;

namespace Application.Features.Tickets.Carts.Pays;

public static class PayCartMapping
{
    public static List<OrderItem> ToListOrderItem(this IEnumerable<CartItem> cartItems)
    {
        return
        [
            .. cartItems.Select(x => new OrderItem()
            {
                Id = x.Id,
                TiketId = x.TiketId,
                CreatedAt = x.CreatedAt,
                Quantity = x.Quantity,
                TotalPrice = x.TotalPrice,
            }),
        ];
    }

    public static PayCartResponse ToPayCartResponse(this Cart cart)
    {
        return new()
        {
            Customer = new()
            {
                FullName = cart.Customer?.FullName,
                Address = cart.Customer?.Address,
                Email = cart.Customer?.Email,
                PhoneNumber = cart.Customer?.PhoneNumber,
            },
            CartStatus = cart.CartStatus,
            Total = cart.Total,
            ShippingFee = cart.ShippingFee,
            ShippingAddress = cart.ShippingAddress,
            PhoneNumber = cart.PhoneNumber,
            CartItems = cart.CartItems?.Select(x => new CartItemProjection()
            {
                Quantity = x.Quantity,
                Ticket = new TicketProjection()
                {
                    EventName = x.Ticket!.EventName,
                    EventDate = x.Ticket.EventDate,
                    Price = x.Ticket.Price,
                },
                TotalPrice = x.TotalPrice,
            }),
        };
    }
}
