using Domain.Aggregates.Carts;
using Domain.Aggregates.Carts.Enums;
using Domain.Aggregates.Customers;

namespace Application.UseCases.Tickets.Carts.Create;

public class CreateCartResponse
{
    public string? ShippingAddress { get; set; }

    public int ShippingFee { get; set; }

    public string? PhoneNumber { get; set; }

    public int Total { get; set; }

    public CartStatus CartStatus { get; set; }

    public string? InvalidReason { get; set; }

    public Customer? Customer { get; set; }

    public IEnumerable<CartItemResponse>? CartItems { get; set; }
}

public class CustomerResponse
{
    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public string? Email { get; set; }
}

public class CartItemResponse
{
    public int Quantity { get; set; }

    public int TotalPrice { get; set; }

    public TicketResponse? Ticket { get; set; }
}

public class TicketResponse
{
    public string? EventName { get; set; }
    public DateTime EventDate { get; set; }
    public decimal Price { get; set; }
}