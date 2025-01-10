using System.Net;
using Application.Common.Exceptions;
using Application.Features.Common.Projections.Roles;
using Application.Features.Roles.Commands.Create;
using Application.SubcutaneousTests.Extensions;
using AutoFixture;
using Contracts.ApiWrapper;
using FluentAssertions;

namespace Application.SubcutaneousTests.Roles.Commands.Create;

[Collection(nameof(TestingCollectionFixture))]
public class CreateRoleCommandValidatorTest : IAsyncLifetime
{
    private readonly Fixture fixture = new();
    private readonly TestingFixture testingFixture;

    private CreateRoleCommand command;
    private List<RoleClaimModel> roleClaims;

    public CreateRoleCommandValidatorTest(TestingFixture testingFixture)
    {
        this.testingFixture = testingFixture;

        roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name)
            .With(x => x.Description)
            .With(x => x.RoleClaims, roleClaims)
            .Create();
    }

    [Fact]
    public async Task CreateRole_WhenMissingName_ShouldReturnValidationException()
    {
        command.Name = null;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenInvalidLengthName_ShouldReturnValidationException()
    {
        command.Name = new string(fixture.CreateMany<char>(258).ToArray());

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenDuplicatedName_ShouldReturnDuplicatedMessage()
    {
        command.Name = "admin";
        HttpResponseMessage response = await testingFixture.MakeRequestAsync(
            "roles",
            HttpMethod.Post,
            command
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ErrorResponse? errorResponse = await response.ToResponse<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        List<BadRequestError> badRequestErrors = [.. errorResponse!.Errors!];

        BadRequestError firstElement = badRequestErrors[0];

        firstElement.PropertyName.Should().Be("Name");
        List<ReasonTranslation> reasons = [.. firstElement.Reasons];

        reasons[0].Message.Should().Be("role_name_existence");
    }

    [Fact]
    public async Task CreateRole_WhenInvalidLengthDescription_ShouldReturnValidationException()
    {
        command.Description = new string(fixture.CreateMany<char>(10001).ToArray());

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimTypeOfRoleClaim_ShouldReturnValidationException()
    {
        roleClaims.ForEach(claim => claim.ClaimType = null);

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimValueOfRoleClaim_ShouldReturnValidationException()
    {
        roleClaims.ForEach(claim => claim.ClaimValue = null);
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        await testingFixture.SeedingUserAsync();
    }
}
