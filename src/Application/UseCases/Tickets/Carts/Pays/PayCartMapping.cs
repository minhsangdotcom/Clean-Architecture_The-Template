using AutoMapper;
using Domain.Aggregates.Carts;

namespace Application.UseCases.Tickets.Carts.Pays;

public class PayCartMapping : Profile
{
    public PayCartMapping()
    {
        CreateMap<Cart, PayCartResponse>();
    }
}
