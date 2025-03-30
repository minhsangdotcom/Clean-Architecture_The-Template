using System.Net;
using Application.Common.Exceptions;
using Application.Features.Common.Projections.Roles;
using Application.Features.Roles.Commands.Update;
using Application.SubcutaneousTests.Extensions;
using AutoFixture;
using Contracts.ApiWrapper;
using Domain.Aggregates.Roles;
using FluentAssertions;
using Infrastructure.Constants;
using SharedKernel.Common.Messages;

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
        updateRoleCommand.Role.Name = Credential.ADMIN_ROLE;
        UserAddress address = await testingFixture.SeedingRegionsAsync();
        await testingFixture.CreateAdminUserAsync(address);

        var response = await testingFixture.MakeRequestAsync(
            "roles",
            HttpMethod.Post,
            updateRoleCommand.Role
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ErrorResponse? errorResponse = await response.ToResponse<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        List<BadRequestError> badRequestErrors = [.. errorResponse!.Errors!];

        BadRequestError firstElement = badRequestErrors[0];

        firstElement.PropertyName.Should().Be("Name");
        List<ReasonTranslation> reasons = [.. firstElement.Reasons];

        reasons[0]
            .Message.Should()
            .Be(
                Messager
                    .Create<RoleModel>(nameof(Role))
                    .Property(x => x.Name!)
                    .Message(MessageType.Existence)
                    .Build()
                    .Message
            );
    }

    [Fact]
    public async Task CreateRole_WhenInvalidLengthDescription_ShouldReturnValidationException()
    {
        updateRoleCommand.Role.Description = new string([.. fixture.CreateMany<char>(1001)]);
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

    public async Task DisposeAsync() => await Task.CompletedTask;

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();

        updateRoleCommand = RoleMappingExtension.ToUpdateRoleCommand(
            await testingFixture.CreateManagerRoleAsync()
        );
    }
}
