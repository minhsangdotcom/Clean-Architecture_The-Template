using Domain.Common;

namespace Domain.Aggregates.Tickets;

public class Ticket : DefaultEntity
{
    public string EventName { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public decimal Price { get; set; }
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public bool IsSoldOut => AvailableQuantity <= 0;

    public ICollection<Order>? Orders { get; set; }
}