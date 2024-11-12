
namespace Application.UseCases.Projections.Tickets.Carts;

public class CartItemProjection
{
    
    public int Quantity { get; set; }

    public int TotalPrice { get; set; }

    public TicketProjection? Ticket { get; set; }
}
