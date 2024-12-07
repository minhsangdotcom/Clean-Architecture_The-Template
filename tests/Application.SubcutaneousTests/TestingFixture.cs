using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Application.SubcutaneousTests;

public class TestingFixture : IAsyncLifetime
{
    private CustomWebApplicationFactory<Program>? factory;
    private readonly PostgreSqlDatabase database;

    public TestingFixture()
    {
        database = new();
    }

    public async Task DisposeAsync()
    {
        await database.DisposeAsync();
    }

    public async Task InitializeAsync()
    {
        await database.InitialiseAsync();
        var connection = database.GetConnection();
        factory = new(connection);
    }

    public async Task ResetAsync()
    {
        if (database != null)
        {
            await database.ResetAsync();
        }
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        if (factory == null)
        {
            throw new NullReferenceException("factory is null");
        }

        using var scope = factory.Services.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(request);
    }

    public HttpClient CreateClient()
    {
        if (factory == null)
        {
            throw new NullReferenceException("factory is null");
        }
        return factory.CreateClient();
    }
}
