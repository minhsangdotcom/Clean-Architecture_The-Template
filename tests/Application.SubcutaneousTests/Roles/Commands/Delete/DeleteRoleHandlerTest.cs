using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Application.UseCases.Roles.Commands.Delete;
using AutoFixture;
using Contracts.ApiWrapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Roles;
using FluentAssertions;
using Mediator;
using Moq;

namespace Application.SubcutaneousTests.Roles.Commands.Delete;

public class DeleteRoleHandlerTest
{
    private readonly Mock<IRoleManagerService> roleManagerServiceMock;

    private readonly Fixture fixture = new();

    public DeleteRoleHandlerTest()
    {
        roleManagerServiceMock = new();
    }

    [Fact]
    public async Task DeleteRole_WhenInvalidId_ShouldReturnNotFoundException()
    {
        Ulid id = Ulid.NewUlid();

        List<MessageResult> messageResults =
        [
            Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage(),
        ];

        roleManagerServiceMock
            .Setup(x => x.FindByIdAsync(id))
            .ThrowsAsync(new NotFoundException(messageResults));

        var handler = new DeleteRoleHandler(roleManagerServiceMock.Object);
        Func<Task<Unit>> deleteRoleHandler = async () =>
            await handler.Handle(new DeleteRoleCommand(id), CancellationToken.None);

        var result = await deleteRoleHandler
            .Should()
            .ThrowAsync<NotFoundException>(becauseArgs: messageResults);

        ReasonTranslation error = result.And.Errors.First().Reasons.First();
        MessageResult messageResult = messageResults[0];
        error.Should().NotBeNull();

        error.Message.Should().Be(messageResult.Message);
        error.En.Should().Be(messageResult.En);
        error.Vi.Should().Be(messageResult.Vi);
    }

    [Fact]
    public async Task DeleteRole_WhenValidId_ShouldDeleteRole()
    {
        Ulid id = Ulid.NewUlid();

        Role currentRole = fixture.Build<Role>().With(x => x.Id, id).OmitAutoProperties().Create();

        roleManagerServiceMock.Setup(x => x.FindByIdAsync(id)).ReturnsAsync(currentRole);

        roleManagerServiceMock.Setup(x => x.DeleteRoleAsync(currentRole)).Verifiable();

        var handler = new DeleteRoleHandler(roleManagerServiceMock.Object);
        await handler.Handle(new DeleteRoleCommand(id), CancellationToken.None);

        roleManagerServiceMock.Verify(x => x.DeleteRoleAsync(currentRole), Times.Once);
    }
}
