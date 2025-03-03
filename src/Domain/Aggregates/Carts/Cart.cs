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

    public CartStatus CartStatus { get; set; } = CartStatus.Active;

    /// <summary>
    /// record bad request error reason
    /// </summary>
    public string? PersistentError { get; set; }

    public string? PaymentResult { get; set; }

    public bool IsPaid { get; set; }

    public ICollection<CartItem>? CartItems { get; set; }

    protected override bool TryApplyDomainEvent(INotification domainEvent)
    {
        return true;
    }
}
