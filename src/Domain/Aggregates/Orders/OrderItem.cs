using Domain.Aggregates.Tickets;
using Domain.Common;

namespace Domain.Aggregates.Orders;

public class OrderItem : BaseEntity
{
    public Ulid TiketId { get; set; }

    public Ticket? Ticket { get; set; }

    public Ulid OrderId { get; set; }

    public Order? Order { get; set; }

    public int Quantity { get; set; }

    public int TotalPrice { get; set; }
}
