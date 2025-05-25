using Application.Features.Common.Projections.Roles;
using Application.Features.Roles.Commands.Update;
using Application.SubcutaneousTests.Extensions;
using AutoFixture;
using CaseConverter;
using Domain.Aggregates.Roles;
using SharedKernel.Common.Messages;
using SharedKernel.Extensions;
using Shouldly;

namespace Application.SubcutaneousTests.Roles.Commands.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateRoleHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();
    private UpdateRoleCommand updateRoleCommand = new();

    [Fact]
    public async Task UpdateRole_WhenIdNotFound_ShouldReturnNotFoundException()
    {
        // Arrange
        var updatedRole = fixture.Build<RoleUpdateRequest>().Without(x => x.RoleClaims).Create();

        var ulid = Ulid.NewUlid();
        var updateRoleCommand = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, ulid.ToString())
            .With(x => x.UpdateData, updatedRole)
            .Create();

        // Act
        var result = await testingFixture.SendAsync(updateRoleCommand);

        // Assert
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
    public async Task UpdateRole_WhenNoRoleClaims_ShouldUpdateRole()
    {
        // Arrange
        updateRoleCommand.UpdateData.RoleClaims = null;

        // Act
        var result = await testingFixture.SendAsync(updateRoleCommand);
        var response = result.Value!;

        // Assert
        var createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(response.Id);
        createdRole.ShouldNotBeNull();

        var requestData = updateRoleCommand.UpdateData;
        createdRole!.Name.ShouldBe(requestData.Name!.ToScreamingSnakeCase());
        createdRole.Description.ShouldBe(requestData.Description);
        createdRole.RoleClaims?.Count.ShouldBe(0);
    }

    [Fact]
    public async Task UpdateRole_WhenNoDescription_ShouldUpdateRole()
    {
        // Arrange
        updateRoleCommand.UpdateData.Description = null;

        // Act
        var result = await testingFixture.SendAsync(updateRoleCommand);
        var response = result.Value!;

        // Assert
        var createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(response.Id);
        createdRole.ShouldNotBeNull();

        var requestData = updateRoleCommand.UpdateData;
        createdRole!.Name.ShouldBe(requestData.Name!.ToScreamingSnakeCase());
        createdRole.RoleClaims?.Count.ShouldBe(requestData.RoleClaims!.Count);
        createdRole.Description.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateRole_ShouldUpdateRole()
    {
        // Arrange
        var requestData = updateRoleCommand.UpdateData;
        var roleClaims = requestData.RoleClaims!;
        var uer = await testingFixture.CreateManagerUserAsync(roleIds: [Ulid.Parse(updateRoleCommand.RoleId)]);

        // modify the claims collection
        roleClaims.RemoveAt(1);
        roleClaims.Add(new RoleClaimModel { ClaimType = "permission", ClaimValue = "list.user" });
        roleClaims[0].ClaimValue = "create.users";

        requestData.Name = $"name{Guid.NewGuid()}";
        requestData.Description = $"description{Guid.NewGuid()}";

        // Act
        var result = await testingFixture.SendAsync(updateRoleCommand);

        // Assert
        var response = result.Value!;
        var createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(response.Id);
        var userClaims = await testingFixture.FindUserClaimsByRoleAsync(response.Id,
            roleClaims.Select(x => new KeyValuePair<string, string>(x.ClaimType!, x.ClaimValue!)));
        createdRole.ShouldNotBeNull();

        createdRole!.Name.ShouldBe(requestData.Name!.ToScreamingSnakeCase());

        var expectedClaims = roleClaims.Select(rc => new { rc.ClaimType, rc.ClaimValue }).ToList();
        var actualClaims = createdRole
            .RoleClaims!.Select(rc => new { rc.ClaimType, rc.ClaimValue })
            .ToList();
        actualClaims.ShouldBe(expectedClaims!, ignoreOrder: true);
        if (userClaims.Count > 0)
        {
            userClaims.Select(x => new { x.ClaimType, x.ClaimValue }).ShouldBe(actualClaims, ignoreOrder: true);
        }
        createdRole.Description.ShouldBe(requestData.Description);
    }

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        updateRoleCommand = RoleMappingExtension.ToUpdateRoleCommand(
            await testingFixture.CreateAdminRoleAsync()
        );
    }

    public async Task DisposeAsync() => await Task.CompletedTask;
}