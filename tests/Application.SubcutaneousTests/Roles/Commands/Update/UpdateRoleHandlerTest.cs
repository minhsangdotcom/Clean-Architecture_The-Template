using Application.Common.Exceptions;
using Application.Features.Common.Projections.Roles;
using Application.Features.Roles.Commands.Update;
using AutoFixture;
using CaseConverter;
using Contracts.ApiWrapper;
using Domain.Aggregates.Roles;
using FluentAssertions;
using SharedKernel.Common.Messages;

namespace Application.SubcutaneousTests.Roles.Commands.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateRoleHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();
    private UpdateRoleCommand updateRoleCommand = new();

    [Fact]
    public async Task UpdateRole_WhenIdNotFound_ShouldReturnNotFoundException()
    {
        RoleUpdateRequest updatedRole = fixture
            .Build<RoleUpdateRequest>()
            .Without(x => x.RoleClaims)
            .Create();

        Ulid ulid = Ulid.NewUlid();
        UpdateRoleCommand updateRoleCommand = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, ulid.ToString())
            .With(x => x.UpdateData, updatedRole)
            .Create();

        List<MessageResult> messageResults =
        [
            Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage(),
        ];

        var result = await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
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
    public async Task UpdateRole_WhenNoRoleClaims_ShouldUpdateRole()
    {
        updateRoleCommand.UpdateData.RoleClaims = null;

        Result<UpdateRoleResponse> result = await testingFixture.SendAsync(updateRoleCommand);
        UpdateRoleResponse updateRoleResponse = result.Value!;

        Role? createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(
            updateRoleResponse.Id
        );
        createdRole.Should().NotBeNull();
        RoleUpdateRequest RoleUpdateRequest = updateRoleCommand.UpdateData;
        createdRole!.Name.Should().Be(RoleUpdateRequest.Name!.ToSnakeCase().ToUpper());
        createdRole!.Description.Should().Be(RoleUpdateRequest.Description);
        createdRole.RoleClaims.Should().HaveCount(0);
    }

    [Fact]
    public async Task UpdateRole_WhenNoDescription_ShouldUpdateRole()
    {
        updateRoleCommand.UpdateData.Description = null;

        Result<UpdateRoleResponse> result = await testingFixture.SendAsync(updateRoleCommand);
        UpdateRoleResponse updateRoleResponse = result.Value!;

        Role? createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(
            updateRoleResponse.Id
        );
        createdRole.Should().NotBeNull();
        RoleUpdateRequest RoleUpdateRequest = updateRoleCommand.UpdateData;
        createdRole!.Name.Should().Be(RoleUpdateRequest.Name!.ToSnakeCase().ToUpper());
        createdRole
            .RoleClaims.Should()
            .HaveCount(updateRoleCommand.UpdateData.RoleClaims!.Count);
        createdRole!.Description.Should().BeNull();
    }

    [Fact]
    public async Task UpdateRole_ShouldUpdateRole()
    {
        RoleUpdateRequest RoleUpdateRequest = updateRoleCommand.UpdateData;
        List<RoleClaimModel> roleClaims = RoleUpdateRequest.RoleClaims!;
        roleClaims.RemoveAt(1);
        roleClaims.Add(new RoleClaimModel() { ClaimType = "permission", ClaimValue = "list.user" });
        roleClaims[0].ClaimValue = "create.users";
        RoleUpdateRequest.Name = $"name{Guid.NewGuid()}";
        RoleUpdateRequest.Description = $"description{Guid.NewGuid()}";

        Result<UpdateRoleResponse> result = await testingFixture.SendAsync(updateRoleCommand);
        UpdateRoleResponse updateRoleResponse = result.Value!;

        Role? createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(
            updateRoleResponse.Id
        );
        createdRole.Should().NotBeNull();
        createdRole!.Name.Should().Be(RoleUpdateRequest.Name!.ToSnakeCase().ToUpper());
        createdRole
            .RoleClaims!.Select(rc => new { rc.ClaimType, rc.ClaimValue })
            .Should()
            .IntersectWith(roleClaims.Select(rc => new { rc.ClaimType, rc.ClaimValue })!);
        createdRole!.Description.Should().Be(RoleUpdateRequest.Description);
    }

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        Role role = await testingFixture.CreateRoleAsync("admin");
        updateRoleCommand = testingFixture.ToUpdateRoleCommand(role);
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }
}
