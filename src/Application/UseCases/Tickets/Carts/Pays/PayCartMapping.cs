using AutoMapper;
using Domain.Aggregates.Carts;
using Domain.Aggregates.Orders;

namespace Application.UseCases.Tickets.Carts.Pays;

public class PayCartMapping : Profile
{
    public PayCartMapping()
    {
        CreateMap<Cart, PayCartResponse>();
        CreateMap<CartItem, OrderItem>();
    }
}
