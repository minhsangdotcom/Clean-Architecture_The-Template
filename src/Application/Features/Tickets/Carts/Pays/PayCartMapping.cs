using AutoMapper;
using Domain.Aggregates.Carts;
using Domain.Aggregates.Orders;

namespace Application.Features.Tickets.Carts.Pays;

public class PayCartMapping : Profile
{
    public PayCartMapping()
    {
        CreateMap<Cart, PayCartResponse>();
        CreateMap<CartItem, OrderItem>();
    }
}
