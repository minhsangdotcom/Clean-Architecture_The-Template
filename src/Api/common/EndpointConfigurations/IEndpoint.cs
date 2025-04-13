using Api.common.Routers;

namespace Api.common.EndpointConfigurations;

public interface IEndpoint
{
    public EndpointVersion Version { get; }

    public void MapEndpoint(IEndpointRouteBuilder app);
}
