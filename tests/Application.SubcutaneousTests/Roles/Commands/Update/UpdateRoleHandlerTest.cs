using Application.Common.Exceptions;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Roles.Commands.Update;
using AutoFixture;
using CaseConverter;
using Contracts.ApiWrapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Roles;
using FluentAssertions;

namespace Application.SubcutaneousTests.Roles.Commands.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateRoleHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();
    private UpdateRoleCommand updateRoleCommand = new();

    [Fact]
    public async Task UpdateRole_WhenIdNotFound_ShouldReturnNotFoundException()
    {
        UpdateRole updatedRole = fixture.Build<UpdateRole>().Without(x => x.RoleClaims).Create();

        Ulid ulid = Ulid.NewUlid();
        UpdateRoleCommand updateRoleCommand = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, ulid.ToString())
            .With(x => x.Role, updatedRole)
            .Create();

        List<MessageResult> messageResults =
        [
            Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage(),
        ];

        var result = await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
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
    public async Task UpdateRole_WhenNoRoleClaims_ShouldUpdateRole()
    {
        updateRoleCommand.Role.RoleClaims = null;
        var createRoleResponse = await testingFixture.SendAsync(updateRoleCommand);

        Role? createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(
            createRoleResponse.Id
        );
        createdRole.Should().NotBeNull();
        UpdateRole updateRole = updateRoleCommand.Role;
        createdRole!.Name.Should().Be(updateRole.Name!.ToSnakeCase().ToUpper());
        createdRole!.Description.Should().Be(updateRole.Description);
        createdRole.RoleClaims.Should().HaveCount(0);
    }

    [Fact]
    public async Task UpdateRole_WhenNoDescription_ShouldUpdateRole()
    {
        updateRoleCommand.Role.Description = null;
        var createRoleResponse = await testingFixture.SendAsync(updateRoleCommand);

        Role? createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(
            createRoleResponse.Id
        );
        createdRole.Should().NotBeNull();
        UpdateRole updateRole = updateRoleCommand.Role;
        createdRole!.Name.Should().Be(updateRole.Name!.ToSnakeCase().ToUpper());
        createdRole.RoleClaims.Should().HaveCount(updateRoleCommand.Role.RoleClaims!.Count);
        createdRole!.Description.Should().BeNull();
    }

    [Fact]
    public async Task UpdateRole_ShouldUpdateRole()
    {
        UpdateRole updateRole = updateRoleCommand.Role;
        List<RoleClaimModel> roleClaims = updateRole.RoleClaims!;
        for (int i = 0; i < roleClaims!.Count; i++)
        {
            roleClaims[0].ClaimValue = $"ClaimValue{Guid.NewGuid()}";
        }
        updateRole.Name = $"name{Guid.NewGuid()}";
        updateRole.Description = $"description{Guid.NewGuid()}";
        var createRoleResponse = await testingFixture.SendAsync(updateRoleCommand);

        Role? createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(
            createRoleResponse.Id
        );
        createdRole.Should().NotBeNull();
        createdRole!.Name.Should().Be(updateRole.Name!.ToSnakeCase().ToUpper());
        createdRole.RoleClaims.Should().HaveCount(roleClaims.Count);
        createdRole!.Description.Should().Be(updateRole.Description);
    }

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        updateRoleCommand = await testingFixture.CreateRoleAsync("admin", fixture);
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }
}
