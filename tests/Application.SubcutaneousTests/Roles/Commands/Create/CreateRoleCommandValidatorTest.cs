using System.Net;
using System.Net.Http.Json;
using System.Text;
using Application.Common.Exceptions;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Roles.Commands.Create;
using AutoFixture;
using Contracts.ApiWrapper;
using Contracts.Extensions;
using Elastic.Clients.Elasticsearch;
using FluentAssertions;

namespace Application.SubcutaneousTests.Roles.Commands.Create;

[Collection(nameof(TestingCollectionFixture))]
public class CreateRoleCommandValidatorTest(TestingFixture testingFixture) : IAsyncLifetime
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

    [Fact]
    public async Task CreateRole_WhenInvalidLengthName_ShouldReturnValidationException()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        var name = new string(fixture.CreateMany<char>(258).ToArray());
        var command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name, name)
            .With(x => x.RoleClaims, roleClaims)
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenDuplicatedName_ShouldReturnDuplicatedMessage()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        var command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name, "admin")
            .With(x => x.RoleClaims, roleClaims)
            .Create();
        StringContent payload =
            new(
                SerializerExtension.Serialize(command).StringJson,
                Encoding.UTF8,
                "application/json"
            );
        var response = await testingFixture
            .CreateClient()
            .PostAsync("http://localhost:8080/api/roles", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ErrorResponse? errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
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
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        var command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name)
            .With(x => x.RoleClaims, roleClaims)
            .With(x => x.Description, new string(fixture.CreateMany<char>(10001).ToArray()))
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimTypeOfRoleClaim_ShouldReturnValidationException()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).Without(x => x.ClaimType).CreateMany(2).ToList();
        var command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name)
            .With(x => x.RoleClaims, roleClaims)
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimValueOfRoleClaim_ShouldReturnValidationException()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).Without(x => x.ClaimValue).CreateMany(2).ToList();
        var command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name)
            .With(x => x.RoleClaims, roleClaims)
            .Create();

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
    }
}
