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
}
