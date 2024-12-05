using Application.Common.Exceptions;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Roles.Commands.Create;
using AutoFixture;
using FluentAssertions;

namespace Application.SubcutaneousTests.Roles.Commands.Create;

[Collection("CustomWebApplication")]
public class CreateRoleCommandValidatorTest(TestingFixture testingFixture)
    : IClassFixture<TestingFixture>
{
    private readonly Fixture fixture = new();

    [Fact]
    public async Task CreateRole_WhenMissingName_ShouldReturnValidationException()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();

        var command = fixture
            .Build<CreateRoleCommand>()
            .Without(x => x.Name)
            .With(x => x.RoleClaims, roleClaims)
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }
}
