using Application.Common.Exceptions;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Roles.Commands.Create;
using AutoFixture;
using Contracts.ApiWrapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Roles;
using FluentAssertions;
using Mediator;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Application.SubcutaneousTests.Roles.Commands.Create;

[Collection("CustomWebApplication")]
public class CreateRoleCommandValidatorTest : IClassFixture<TestingFixture>, IDisposable
{
    private readonly Mock<ISender> sender = new();
    private readonly Fixture fixture = new();

    private readonly TestingFixture testingFixture;

    public CreateRoleCommandValidatorTest(TestingFixture testingFixture)
    {
        this.testingFixture = testingFixture;
    }

    [Fact]
    public async void CreateRole_Return_Bad_Request_When_Miss_Name()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();

        var command = fixture
            .Build<CreateRoleCommand>()
            .Without(x => x.Name)
            .With(x => x.RoleClaims, roleClaims)
            .Create();

        var result = await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();

        var error = result.BeOfType<ErrorResponse>();

        error.Which.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        error
            .Which.Errors.Should()
            .BeOfType(typeof(IEnumerable<BadRequestError>))
            .Should()
            .Be(
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Negative()
                    .Message(MessageType.Null)
                    .Build()
            );
    }

    public void Dispose()
    {
        return;
    }
}
