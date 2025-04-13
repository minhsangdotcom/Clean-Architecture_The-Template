using System.Net;
using Application.Common.Exceptions;
using Application.Features.Roles.Commands.Update;
using Application.SubcutaneousTests.Extensions;
using AutoFixture;
using Contracts.ApiWrapper;
using Domain.Aggregates.Roles;
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
        updateRoleCommand.UpdateData.Name = null;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateRole_WhenInvalidLengthName_ShouldReturnValidationException()
    {
        updateRoleCommand.UpdateData.Name = new string(fixture.CreateMany<char>(258).ToArray());
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateRole_WhenDuplicatedName_ShouldReturnDuplicatedMessage()
    {
        const string ROLE_NAME = "userTest";
        Role role = await testingFixture.CreateRoleAsync(ROLE_NAME);

        updateRoleCommand.UpdateData.Name = role.Name;

        var response = await testingFixture.MakeRequestAsync(
            "roles",
            HttpMethod.Post,
            updateRoleCommand.UpdateData
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ErrorResponse? errorResponse = await response.ToResponse<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        List<InvalidParam> badRequestErrors = [.. errorResponse!.Errors!];

        InvalidParam firstElement = badRequestErrors[0];

        firstElement.PropertyName.Should().Be("Name");
        List<ErrorReason> reasons = [.. firstElement.Reasons];

        reasons[0].Message.Should().Be("role_name_existence");
    }

    [Fact]
    public async Task CreateRole_WhenInvalidLengthDescription_ShouldReturnValidationException()
    {
        updateRoleCommand.UpdateData.Description = new string(
            fixture.CreateMany<char>(1001).ToArray()
        );
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimTypeOfRoleClaim_ShouldReturnValidationException()
    {
        updateRoleCommand.UpdateData.RoleClaims!.ForEach(x => x.ClaimType = null);
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateRoleCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimValueOfRoleClaim_ShouldReturnValidationException()
    {
        updateRoleCommand.UpdateData.RoleClaims!.ForEach(x => x.ClaimValue = null);
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

        Role role = await testingFixture.CreateRoleAsync("managerTest");
        updateRoleCommand = testingFixture.ToUpdateRoleCommand(role);
        await testingFixture.SeedingUserAsync();
    }
}
