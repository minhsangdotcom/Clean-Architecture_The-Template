using Domain.Aggregates.Tickets;
using Domain.Common;

namespace Domain.Aggregates.Carts;

public class CartItem : BaseEntity
{
    public Ulid TiketId { get; set; }

    public Ticket? Ticket { get; set; }

     public Ulid CartId { get; set; }

    public Cart? Cart { get; set; }

    public int Quantity { get; set; }

    public int TotalPrice { get; set; }
}
