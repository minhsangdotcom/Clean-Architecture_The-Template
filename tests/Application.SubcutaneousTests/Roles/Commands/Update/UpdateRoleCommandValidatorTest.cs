using System.Net;
using System.Net.Http.Json;
using System.Text;
using Application.Common.Exceptions;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Roles.Commands.Update;
using AutoFixture;
using Contracts.ApiWrapper;
using Contracts.Extensions;
using FluentAssertions;

namespace Application.SubcutaneousTests.Roles.Commands.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateRoleCommandValidatorTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();

    private UpdateRoleCommand updateRoleCommand = new();

    [Fact]
    public async Task UpdateRole_WhenNoName_ShouldReturnValidationException()
    {
        updateRoleCommand.Role.Name = null;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateRole_WhenInvalidLengthName_ShouldReturnValidationException()
    {
        updateRoleCommand.Role.Name = new string(fixture.CreateMany<char>(258).ToArray());
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateRole_WhenDuplicatedName_ShouldReturnDuplicatedMessage()
    {
        const string ROLE_NAME = "userTest";
        _ = await testingFixture.CreateRoleAsync(ROLE_NAME, fixture);

        updateRoleCommand.Role.Name = ROLE_NAME;

        StringContent payload =
            new(
                SerializerExtension.Serialize(updateRoleCommand.Role).StringJson,
                Encoding.UTF8,
                "application/json"
            );
        var response = await testingFixture
            .CreateClient()
            .PutAsync($"http://localhost:8080/api/roles/{updateRoleCommand.RoleId}", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ErrorResponse? errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        List<BadRequestError> badRequestErrors = [.. errorResponse!.Errors!];

        BadRequestError firstElement = badRequestErrors[0];

        firstElement.PropertyName.Should().Be("Role.Name");
        List<ReasonTranslation> reasons = [.. firstElement.Reasons];

        reasons[0].Message.Should().Be("role_name_existence");
    }

    [Fact]
    public async Task CreateRole_WhenInvalidLengthDescription_ShouldReturnValidationException()
    {
        updateRoleCommand.Role.Description = new string(fixture.CreateMany<char>(1001).ToArray());
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimTypeOfRoleClaim_ShouldReturnValidationException()
    {
        updateRoleCommand.Role.RoleClaims!.ForEach(x => x.ClaimType = null);
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimValueOfRoleClaim_ShouldReturnValidationException()
    {
        updateRoleCommand.Role.RoleClaims!.ForEach(x => x.ClaimValue = null);
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
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

        updateRoleCommand = await testingFixture.CreateRoleAsync("managerTest", fixture);
    }
}
