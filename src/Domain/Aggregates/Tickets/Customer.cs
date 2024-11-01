using Domain.Common;

namespace Domain.Aggregates.Tickets;

public class Customer : DefaultEntity
{
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public ICollection<Order>? Orders { get; set; }
}
