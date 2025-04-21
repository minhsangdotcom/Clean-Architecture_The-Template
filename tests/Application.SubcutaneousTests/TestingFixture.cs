using Application.SubcutaneousTests.Extensions;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Application.SubcutaneousTests;

public partial class TestingFixture : IAsyncLifetime
{
    private CustomWebApplicationFactory<Program>? factory;
    private readonly PostgreSqlDatabase database;

    private const string BASE_URL = "http://localhost:8080/api/";
    private HttpClient? Client;

    private static Ulid UserId;

    public TestingFixture()
    {
        database = new();
    }

    public async Task DisposeAsync()
    {
        await database.DisposeAsync();
        if (factory != null)
        {
            await factory!.DisposeAsync();
        }

        if (Client != null)
        {
            Client!.Dispose();
        }
    }

    public async Task InitializeAsync()
    {
        await database.InitialiseAsync();
        var connection = database.GetConnection();
        string environmentName = database.GetEnvironmentVariable();
        factory = new(connection, environmentName);
        CreateClient();
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
        factory.ThrowIfNull();
        using var scope = factory!.Services.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(request);
    }

    public async Task<HttpResponseMessage> MakeRequestAsync(
        string uriString,
        HttpMethod method,
        object payload,
        string? contentType = null
    )
    {
        if (Client == null)
        {
            throw new Exception($"{nameof(Client)} is null");
        }

        var loginPayload = new { Username = "super.admin", Password = "Admin@123" };
        HttpResponseMessage httpResponse = await Client.CreateRequestAsync(
            $"{BASE_URL}users/login",
            HttpMethod.Post,
            loginPayload
        );
        var response = await httpResponse.ToResponse<Response<LoginResponse>>();

        return await Client.CreateRequestAsync(
            $"{BASE_URL}{uriString}",
            method,
            payload,
            contentType,
            response!.Results!.Token
        );
    }

    private void CreateClient()
    {
        factory.ThrowIfNull();
        Client = factory!.CreateClient();
    }

    public static Ulid GetUserId() => UserId;

    public static void RemoveUserId() => UserId = Ulid.Empty;
}

record LoginResponse(string Token, string Refresh);

public class Response<T>
{
    public T? Results { get; set; }
}
