using Application.Common.Interfaces.UnitOfWorks;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Roles.Commands.Create;
using Application.UseCases.Roles.Commands.Update;
using AutoFixture;
using Domain.Aggregates.Roles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.SubcutaneousTests;

public partial class TestingFixture
{
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
