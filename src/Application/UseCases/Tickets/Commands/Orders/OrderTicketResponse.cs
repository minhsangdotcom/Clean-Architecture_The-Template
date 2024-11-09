using Domain.Aggregates.Tickets;

namespace Application.UseCases.Tickets.Commands.Orders;

public class OrderTicketResponse
{
    public Ulid TicketId { get; set; }

    public int Quantity { get; set; }

    public int Total { get; set; }

    public OrderStatus OrderStatus { get; set; } = OrderStatus.Pending;
}
