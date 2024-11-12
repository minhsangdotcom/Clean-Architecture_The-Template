using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Contracts.Routers;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace Application.UseCases.Tickets.Carts.Pays;

public class PayCartCommand
{
    public Ulid CustomerId { get; set; }
}

public class PayCartRequest
{
    [FromRoute(Name = nameof(Router.Id))]
    public string? CartId { get; set; }

    [FromBody]
    public PayCartCommand? Payload { get; set; }
}

public class PayCartPayload
    : QueueRequest<PayCartRequest>,
        IRequest<QueueResponse<PayCartResponse>>;
