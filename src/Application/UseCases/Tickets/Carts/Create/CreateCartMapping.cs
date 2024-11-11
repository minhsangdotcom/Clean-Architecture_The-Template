using AutoMapper;
using Domain.Aggregates.Carts;
using Domain.Aggregates.Customers;
using Domain.Aggregates.Tickets;

namespace Application.UseCases.Tickets.Carts.Create;

public class CreateCartMapping : Profile
{
    public CreateCartMapping()
    {
        CreateMap<CreateCartCommand, Cart>();
        CreateMap<CartItemCommand, CartItem>();

        CreateMap<Cart, CreateCartResponse>();
        CreateMap<Customer, CustomerResponse>();
        CreateMap<CartItem, CartItemResponse>();
        CreateMap<Ticket, TicketResponse>();
    }
}
