using Api.common.Documents;
using Api.common.EndpointConfigurations;
using Api.common.Results;
using Api.common.Routers;
using Application.Features.AuditLogs.Queries;
using Contracts.ApiWrapper;
using Contracts.Dtos.Responses;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

namespace Api.Endpoints.AuditLogs;

public class ListAuditLogEndpoint : IEndpoint
{
    public EndpointVersion Version => EndpointVersion.One;

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(Router.AuditLogRoute.AuditLog, HandleAsync)
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Summary = "Get list of audit logs",
                Description = "Returns a list of audit logs",
                Tags = [new OpenApiTag() { Name = Router.AuditLogRoute.Tags }],
                Parameters = operation.AddDocs(),
            });
    }

    private async Task<
        Results<Ok<ApiResponse<PaginationResponse<ListAuditlogResponse>>>, ProblemHttpResult>
    > HandleAsync(
        ListAuditlogQuery request,
        [FromServices] ISender sender,
        CancellationToken cancellationToken = default
    )
    {
        var result = await sender.Send(request, cancellationToken);
        return result.ToResult();
    }
}
