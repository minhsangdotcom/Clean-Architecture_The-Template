using Application.Common.Exceptions;
using Application.Features.Roles.Commands.Delete;
using Application.Features.Roles.Commands.Update;
using AutoFixture;
using Contracts.ApiWrapper;
using Domain.Aggregates.Roles;
using FluentAssertions;
using SharedKernel.Common.Messages;

namespace Application.SubcutaneousTests.Roles.Commands.Delete;

[Collection(nameof(TestingCollectionFixture))]
public class DeleteRoleHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();

    private Ulid id;

    [Fact]
    public async Task DeleteRole_WhenInvalidId_ShouldReturnNotFoundException()
    {
        List<MessageResult> messageResults =
        [
            Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage(),
        ];
        Ulid notFoundId = Ulid.NewUlid();

        var result = await FluentActions
            .Invoking(() => testingFixture.SendAsync(new DeleteRoleCommand(notFoundId)))
            .Should()
            .ThrowAsync<NotFoundException>(becauseArgs: messageResults);

        ErrorReason error = result.And.Errors.First().Reasons.First();
        MessageResult messageResult = messageResults[0];
        error.Should().NotBeNull();

        error.Message.Should().Be(messageResult.Message);
        error.En.Should().Be(messageResult.En);
        error.Vi.Should().Be(messageResult.Vi);
    }

    [Fact]
    public async Task DeleteRole_WhenValidId_ShouldDeleteRole()
    {
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(new DeleteRoleCommand(id)))
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
        Role role = await testingFixture.CreateRoleAsync("admin");
        UpdateRoleCommand response = testingFixture.ToUpdateRoleCommand(role);
        id = Ulid.Parse(response.RoleId);
    }
}
