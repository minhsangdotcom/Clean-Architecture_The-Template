using Contracts.Dtos.Responses;
using Mediator;

namespace Application.UseCases.Tickets.Commands.Orders;

public class OrderTicketCommand
{
    public Ulid TicketId { get; set; }

    public int Quantity { get; set; }
}
