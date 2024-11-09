using Domain.Aggregates.Customers;
using Domain.Aggregates.Orders.Enums;
using Domain.Common;
using Mediator;

namespace Domain.Aggregates.Orders;

public class Order : AggregateRoot
{
    public Ulid CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int ShippingFee { get; set; }

    public string? ShippingAddress { get; set; }
    public string? PhoneNumber { get; set; }
    public int TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Process;

    public ICollection<OrderItem>? OrderItems { get; set; }

    protected override bool TryApplyDomainEvent(INotification domainEvent)
    {
        throw new NotImplementedException();
    }
}