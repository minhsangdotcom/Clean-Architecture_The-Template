using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Mediator;

namespace Application.Features.Tickets.Carts.Pays;

public class PayCartRequest
{
    public string? CartId { get; set; }

    public Ulid CustomerId { get; set; }
}

public class CartData
{
    public Ulid CustomerId { get; set; }
}

public class PayCartPayload
    : QueueRequest<PayCartRequest>,
        IRequest<QueueResponse<PayCartResponse>>;
