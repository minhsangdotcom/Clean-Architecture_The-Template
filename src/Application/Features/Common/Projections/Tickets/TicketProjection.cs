namespace Application.Features.Common.Projections.Tickets;

public class TicketProjection
{
    public string? EventName { get; set; }
    public DateTime EventDate { get; set; }
    public decimal Price { get; set; }
}
