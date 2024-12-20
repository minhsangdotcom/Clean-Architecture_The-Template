using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.UnitOfWorks;
using Application.SubcutaneousTests.Extensions;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Infrastructure.Constants;
using Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Application.SubcutaneousTests;

public partial class TestingFixture : IAsyncLifetime
{
    private CustomWebApplicationFactory<Program>? factory;
    private readonly PostgreSqlDatabase database;

    private const string BASE_URL = "http://localhost:8080/api/";

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
        factory.ThrowIfNull();
        using var scope = factory!.Services.CreateScope();
        ISender sender = scope.ServiceProvider.GetRequiredService<ISender>();
        return await sender.Send(request);
    }

    public async Task<User> SeedingUserAsync()
    {
        factory.ThrowIfNull();
        var scope = factory!.Services.CreateScope();
        IUnitOfWork unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        IRoleManagerService roleManagerService =
            factory.Services.GetRequiredService<IRoleManagerService>();
        IUserManagerService userManagerService =
            factory.Services.GetRequiredService<IUserManagerService>();
        User user = await unitOfWork
            .Repository<User>()
            .AddAsync(
                new(
                    "admin",
                    "admin",
                    "admin",
                    "$2a$10$d56gyiY.bfDIUMtlVzN.PeLCIct3gh3vkgz.eyc0Gx1YiC881/Yp6",
                    "admin.admin@admin.com.vn",
                    "0925123123"
                )
            );
        await unitOfWork.SaveAsync();
        Role role =
            new()
            {
                Name = "ADMIN",
                Description = "Admin role",
                RoleClaims = Credential
                    .ADMIN_CLAIMS.Select(x => new RoleClaim()
                    {
                        ClaimType = x.Key,
                        ClaimValue = x.Value,
                    })
                    .ToArray(),
            };
        await roleManagerService.CreateRoleAsync(role);
        await userManagerService.AddRoleToUserAsync(user, [role.Id]);
        return user;
    }

    public async Task<HttpResponseMessage> MakeRequestAsync(
        string uriString,
        HttpMethod method,
        object payload,
        string? contentType = null
    )
    {
        using HttpClient httpClient = CreateClient();
        var loginPayload = new { Username = "admin", Password = "Admin@123" };
        HttpResponseMessage httpResponse = await httpClient.CreateRequestAsync(
            $"{BASE_URL}login",
            HttpMethod.Post,
            loginPayload
        );
        var response = await httpResponse.ToResponse<Response<LoginResponse>>();

        using var client = CreateClient();
        return await client.CreateRequestAsync(
            $"{BASE_URL}{uriString}",
            method,
            payload,
            contentType,
            response!.Results!.Token
        );
    }

    private HttpClient CreateClient()
    {
        factory.ThrowIfNull();
        return factory!.CreateClient();
    }
}

record LoginResponse(string Token, string Refresh);

public class Response<T>
{
    public T? Results { get; set; }
}
