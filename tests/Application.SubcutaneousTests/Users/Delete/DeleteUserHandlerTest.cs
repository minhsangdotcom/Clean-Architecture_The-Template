using Application.Common.Exceptions;
using Application.Features.Users.Commands.Delete;
using AutoFixture;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users;
using FluentAssertions;
using SharedKernel.Common.Messages;

namespace Application.SubcutaneousTests.Users.Delete;

[Collection(nameof(TestingCollectionFixture))]
public class DeleteUserHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();

    private Ulid? id;

    [Fact]
    private async Task DeleteUser_WhenIdNotfound_ShouldThrowNotFoundException()
    {
        Result<string> result = await testingFixture.SendAsync(
            new DeleteUserCommand(Ulid.NewUlid())
        );
        var expectedMessage = Messager
            .Create<User>()
            .Message(MessageType.Found)
            .Negative()
            .BuildMessage();
        result.Error.Should().NotBeNull();
        result.Error.Status.Should().Be(404);
        result.Error.ErrorMessage.Should().BeEquivalentTo(expectedMessage);
    }

    [Fact]
    private async Task DeleteUser_WhenIdNotfound_ShouldDeleteSuccess()
    {
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(new DeleteUserCommand(id!.Value)))
            .Should()
            .NotThrowAsync();
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        UserAddress address = await testingFixture.SeedingRegionsAsync();
        User user = await testingFixture.CreateNormalUserAsync(address);
        id = user.Id;
    }
}
