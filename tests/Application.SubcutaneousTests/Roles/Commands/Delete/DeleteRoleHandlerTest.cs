using Application.Features.Roles.Commands.Delete;
using Application.SubcutaneousTests.Extensions;
using Contracts.ApiWrapper;
using Domain.Aggregates.Roles;
using SharedKernel.Common.Messages;
using Shouldly;

namespace Application.SubcutaneousTests.Roles.Commands.Delete;

[Collection(nameof(TestingCollectionFixture))]
public class DeleteRoleHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private Ulid id;

    [Fact]
    public async Task DeleteRole_WhenInvalidId_ShouldReturnNotFoundException()
    {
        Ulid notFoundId = Ulid.NewUlid();

        Result<string> result = await testingFixture.SendAsync(new DeleteRoleCommand(notFoundId));

        var expectedMessage = Messager
            .Create<Role>()
            .Message(MessageType.Found)
            .Negative()
            .BuildMessage();

        result.Error.ShouldNotBeNull();
        result.Error.Status.ShouldBe(404);
        result.Error.ErrorMessage.ShouldBe(expectedMessage, new MessageResultComparer());
    }

    [Fact]
    public async Task DeleteRole_WhenValidId_ShouldDeleteRole()
    {
        var result = await testingFixture.SendAsync(new DeleteRoleCommand(id));
        result.Error.ShouldBeNull();
    }

    public async Task DisposeAsync() => await Task.CompletedTask;

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        Role role = await testingFixture.CreateAdminRoleAsync();
        id = role.Id;
    }
}
