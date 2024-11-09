namespace Domain.Aggregates.Orders.Enums;

public enum OrderStatus
{
    Process = 1,
    Delivering = 2,
    Shipped = 3,
    Cancel = 4,
    Refund = 5,
}
