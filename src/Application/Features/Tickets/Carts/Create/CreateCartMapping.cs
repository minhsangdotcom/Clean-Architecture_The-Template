using Application.Features.Common.Projections.Tickets.Carts;
using Domain.Aggregates.Carts;
using Domain.Aggregates.Customers;
using RazorLight.Extensions;

namespace Application.Features.Tickets.Carts.Create;

public static class CreateCartMapping
{
    public static Cart ToCart(this CreateCartCommand command)
    {
        return new()
        {
            CustomerId = command.CustomerId,
            ShippingAddress = command.ShippingAddress,
            ShippingFee = command.ShippingFee,
            PhoneNumber = command.PhoneNumber,
            Total = command.Total,
            CartItems = command
                .CartItems?.Select(x => new CartItem()
                {
                    TiketId = x.TiketId,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice,
                })
                .ToList(),
        };
    }

    public static CreateCartResponse ToCreateCartResponse(this Cart cart)
    {
        return new()
        {
            PhoneNumber = cart.PhoneNumber,
            ShippingFee = cart.ShippingFee,
            ShippingAddress = cart.ShippingAddress,
            CartStatus = cart.CartStatus,
            Total = cart.Total,
            Customer = new()
            {
                FullName = cart.Customer?.FullName,
                Email = cart.Customer?.Email,
                Address = cart.Customer?.Address,
                PhoneNumber = cart.Customer?.PhoneNumber,
            },
            CartItems = cart.CartItems?.Select(x => new CartItemProjection()
            {
                Quantity = x.Quantity,
                Ticket = new()
                {
                    EventName = x.Ticket?.EventName,
                    Price = x.Ticket!.Price,
                    EventDate = x.Ticket.EventDate,
                },
                TotalPrice = x.TotalPrice,
            }),
        };
    }
}
