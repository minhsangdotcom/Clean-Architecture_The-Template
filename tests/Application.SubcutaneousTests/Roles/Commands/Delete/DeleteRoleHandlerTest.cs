using Application.Features.Roles.Commands.Delete;
using Contracts.ApiWrapper;
using Domain.Aggregates.Roles;
using FluentAssertions;
using SharedKernel.Common.Messages;

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

        result.Error.Should().NotBeNull();
        result.Error.Status.Should().Be(404);
        result.Error.ErrorMessage.Should().BeEquivalentTo(expectedMessage);
    }

    [Fact]
    public async Task DeleteRole_WhenValidId_ShouldDeleteRole()
    {
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(new DeleteRoleCommand(id)))
            .Should()
            .NotThrowAsync();
    }

    public async Task DisposeAsync() => await Task.CompletedTask;

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        Role role = await testingFixture.CreateAdminRoleAsync();
        id = role.Id;
    }
}
