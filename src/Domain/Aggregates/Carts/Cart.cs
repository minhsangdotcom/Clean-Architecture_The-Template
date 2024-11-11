using Domain.Aggregates.Carts.Enums;
using Domain.Aggregates.Customers;
using Domain.Common;
using Mediator;

namespace Domain.Aggregates.Carts;

public class Cart : AggregateRoot
{
    public Ulid CustomerId { get; set; }
    
    public Customer? Customer { get; set; }

    public string? ShippingAddress { get; set; }

    public int ShippingFee { get; set; }

    public string? PhoneNumber { get; set; }

    public int Total { get; set; }

    public CartStatus CartStatus { get; set; }

    public string? InvalidReason { get; set; }

    public ICollection<CartItem>? CartItems { get; set; }

    protected override bool TryApplyDomainEvent(INotification domainEvent)
    {
        return true;
    }
}
