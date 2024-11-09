using Domain.Aggregates.Carts;
using Domain.Aggregates.Orders;
using Domain.Common;

namespace Domain.Aggregates.Customers;

public class Customer : BaseEntity
{
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }

    public ICollection<Order>? Orders { get; set; }
    public ICollection<Cart>? Carts { get; set; }
}
