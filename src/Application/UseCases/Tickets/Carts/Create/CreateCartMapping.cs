using Application.UseCases.Projections.Customers;
using Application.UseCases.Projections.Tickets;
using Application.UseCases.Projections.Tickets.Carts;
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
        CreateMap<Customer, CustomerProjection>();
        CreateMap<CartItem, CartItemProjection>();
        CreateMap<Ticket, TicketProjection>();
    }
}
