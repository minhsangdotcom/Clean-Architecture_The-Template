using Domain.Aggregates.Carts;
using Domain.Aggregates.Orders;
using Domain.Common;

namespace Domain.Aggregates.Tickets;

public class Ticket : BaseEntity
{
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal Price { get; set; }
    public int TotalQuantity { get; set; }
    public int UsedQuantity { get; set; }

    public ICollection<CartItem>? CartItems { get; set; }

    public ICollection<OrderItem>? OrderItems { get; set; }
}