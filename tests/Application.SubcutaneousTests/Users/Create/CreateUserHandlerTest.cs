using AutoFixture;

namespace Application.SubcutaneousTests.Users.Create;

[Collection(nameof(TestingCollectionFixture))]
public class CreateUserHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();

    private Ulid roleId;

    // [Fact]
    // private Task CreateUser_WhenNoCustomClaim_ShouldCreateSuccess()
    // {

    // }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        await testingFixture.SeedingRegionsAsync();
        var response = await testingFixture.CreateRoleAsync("adminTest");
        roleId = response.Id;
        await testingFixture.SeedingUserAsync();
    }
}
