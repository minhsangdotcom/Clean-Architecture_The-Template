using Application.Common.Interfaces.UnitOfWorks;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Specifications;
using Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Application.SubcutaneousTests;

public partial class TestingFixture
{
    public async Task SeedingRegionsAsync()
    {
        using var scope = factory!.Services.CreateScope();
        var provider = scope.ServiceProvider;
        await RegionDataSeeding.SeedingAsync(provider);
    }

    public async Task<User?> FindUserByIdAsync(Ulid userId)
    {
        using var scope = factory!.Services.CreateScope();
        IUnitOfWork? unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();

        return await unitOfWork!
            .Repository<User>()
            .FindByConditionAsync(new GetUserByIdSpecification(userId));
    }
}
