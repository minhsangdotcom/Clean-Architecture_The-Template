namespace Domain.Aggregates.Carts.Enums;

public enum CartStatus
{
    Active = 1,
    Progressing = 2,
    PaymentPending = 3,
    Paid = 4,
    Expired = 5,
    Failed = 6,
}
