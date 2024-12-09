using Application.Common.Interfaces.UnitOfWorks;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Roles.Commands.Create;
using Application.UseCases.Roles.Commands.Update;
using AutoFixture;
using Domain.Aggregates.Roles;
using Mediator;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Role?> FindRoleByIdAsync(Ulid id)
    {
        if (factory == null)
        {
            throw new NullReferenceException("factory is null");
        }

        using var scope = factory.Services.CreateScope();
        IDbContext dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
        return await dbContext.Set<Role>().Where(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Role?> FindRoleByIdIncludeRoleClaimsAsync(Ulid id)
    {
        if (factory == null)
        {
            throw new NullReferenceException("factory is null");
        }

        using var scope = factory.Services.CreateScope();
        IDbContext dbContext = scope.ServiceProvider.GetRequiredService<IDbContext>();
        return await dbContext
            .Set<Role>()
            .Where(x => x.Id == id)
            .Include(x => x.RoleClaims)
            .FirstOrDefaultAsync();
    }

    public async Task<UpdateRoleCommand> CreateRoleAsync(string roleName, Fixture fixture)
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        CreateRoleCommand createRoleCommand = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name, roleName)
            .With(x => x.RoleClaims, roleClaims)
            .Create();

        CreateRoleResponse createRoleResponse = await SendAsync(createRoleCommand);
        return new()
        {
            RoleId = createRoleResponse.Id.ToString(),
            Role = new UpdateRole()
            {
                Name = createRoleResponse.Name,
                Description = createRoleResponse.Description,
                RoleClaims = createRoleResponse
                    .RoleClaims!.Select(x => new RoleClaimModel()
                    {
                        ClaimType = x.ClaimType,
                        ClaimValue = x.ClaimValue,
                        Id = x.Id,
                    })
                    .ToList(),
            },
        };
    }
}
