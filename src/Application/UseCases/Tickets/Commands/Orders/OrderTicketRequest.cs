using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.UseCases.Tickets.Commands.Orders;

public class OrderTicketRequest : QueueRequest<OrderTicket>, IRequest<QueueResponse<OrderTicketResponse>>
{
}

public class OrderTicket
{
    public Ulid UserId { get; set; }
    public Ulid TicketId { get; set; }
    public int Quantity { get; set; }
    public DateTimeOffset RequestTime { get; set; } = DateTimeOffset.UtcNow;
    public PurchaseStatus PurchaseStatus { get; set; }
}