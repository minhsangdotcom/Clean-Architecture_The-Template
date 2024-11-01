using Domain.Aggregates.Users;
using Domain.Common;

namespace Domain.Aggregates.Tickets;

public class Order : DefaultEntity
{
    public Ulid CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public Ulid TicketId { get; set; }
    public Ticket? Ticket { get; set; }

    public int Quantity { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Completed;
}

public enum OrderStatus
{
    Pending,
    Completed,
    Cancelled,
    Failed
}