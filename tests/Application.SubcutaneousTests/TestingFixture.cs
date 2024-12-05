using Mediator;
using Microsoft.Extensions.DependencyInjection;
namespace Application.SubcutaneousTests;

public class TestingFixture
{
    private readonly CustomWebApplicationFactory<Program> factory = new();

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = factory.Services.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(request);
    }
}
