using Application.Features.Common.Projections.Roles;
using Application.Features.Roles.Commands.Create;
using AutoFixture;
using CaseConverter;
using Domain.Aggregates.Roles;
using FluentAssertions;

namespace Application.SubcutaneousTests.Roles.Commands.Create;

[Collection(nameof(TestingCollectionFixture))]
public class CreateRoleHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();

    [Fact]
    public async Task CreateRole_WhenNoRoleClaims_ShouldCreateRole()
    {
        CreateRoleCommand command = fixture
            .Build<CreateRoleCommand>()
            .Without(x => x.RoleClaims)
            .Create();
        CreateRoleResponse createRoleResponse = await testingFixture.SendAsync(command);

        Role? createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(
            createRoleResponse.Id
        );
        createdRole.Should().NotBeNull();
        createdRole!.Name.Should().Be(command.Name.ToSnakeCase().ToUpper());
        createdRole!.Description.Should().Be(command.Description);
        createdRole.RoleClaims.Should().HaveCount(0);
    }

    [Fact]
    public async Task CreateRole_WhenNoDescription_ShouldCreateRole()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        CreateRoleCommand command = fixture
            .Build<CreateRoleCommand>()
            .Without(x => x.Description)
            .With(x => x.RoleClaims, roleClaims)
            .Create();

        CreateRoleResponse createRoleResponse = await testingFixture.SendAsync(command);

        Role? createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(
            createRoleResponse.Id
        );
        createdRole.Should().NotBeNull();
        createdRole!.Name.Should().Be(command.Name.ToSnakeCase().ToUpper());
        createdRole.Description.Should().BeNull();
        createdRole.RoleClaims.Should().HaveCount(roleClaims.Count);
    }

    [Fact]
    public async Task CreateRole_ShouldCreateRole()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        CreateRoleCommand command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.RoleClaims, roleClaims)
            .Create();

        CreateRoleResponse createRoleResponse = await testingFixture.SendAsync(command);

        Role? createdRole = await testingFixture.FindRoleByIdIncludeRoleClaimsAsync(
            createRoleResponse.Id
        );
        createdRole.Should().NotBeNull();
        createdRole!.Name.Should().Be(command.Name.ToSnakeCase().ToUpper());
        createdRole!.Description.Should().Be(command.Description);
        createdRole.RoleClaims.Should().HaveCount(roleClaims.Count);
    }

    public async Task InitializeAsync() => await testingFixture.ResetAsync();

    public async Task DisposeAsync() => await Task.CompletedTask;
}
