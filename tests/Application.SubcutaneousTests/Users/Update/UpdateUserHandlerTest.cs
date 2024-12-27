using Application.Common.Exceptions;
using Application.Features.Users.Commands.Update;
using AutoFixture;
using Domain.Aggregates.Users;
using FluentAssertions;

namespace Application.SubcutaneousTests.Users.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateUserHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();

    private UpdateUserCommand updateUserCommand = new();

    [Fact]
    private async Task UpdateUser_WhenIdNotfound_ShouldCreateSuccess()
    {
        updateUserCommand.UserId = Ulid.NewUlid().ToString();
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<NotFoundException>();
    }

    [Fact]
    private async Task UpdateUser_WhenNoCustomClaim_ShouldCreateSuccess()
    {
        updateUserCommand.User!.UserClaims = null;

        UpdateUserResponse response = await testingFixture.SendAsync(updateUserCommand);
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, updateUserCommand);
    }

    [Fact]
    private async Task UpdateUser_WhenNoAvatar_ShouldCreateSuccess()
    {
        updateUserCommand.User!.Avatar = null;
        UpdateUserResponse response = await testingFixture.SendAsync(updateUserCommand);
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, updateUserCommand);
    }

    [Fact]
    private async Task UpdateUser_WhenNoDayOfBirth_ShouldCreateSuccess()
    {
        updateUserCommand.User!.DayOfBirth = null;
        UpdateUserResponse response = await testingFixture.SendAsync(updateUserCommand);
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, updateUserCommand);
    }

    [Fact]
    private async Task UpdateUser_ShouldCreateSuccess()
    {
        UpdateUserResponse response = await testingFixture.SendAsync(updateUserCommand);
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, updateUserCommand);
    }

    private void AssertUser(User? user, UpdateUserCommand updateUserCommand)
    {
        UpdateUser updateUser = updateUserCommand.User!;
        user.Should().NotBeNull();
        user!.FirstName.Should().Be(updateUser.FirstName);
        user!.LastName.Should().Be(updateUser.LastName);
        user!.Email.Should().Be(updateUser.Email);
        user!.PhoneNumber.Should().Be(updateUser.PhoneNumber);
        user!.Address!.Province!.Id.Should().Be(updateUser.ProvinceId);
        user!.Address!.District!.Id.Should().Be(updateUser.DistrictId);

        if (updateUser.Avatar != null)
        {
            user.Avatar.Should().NotBeNull();
        }
        else
        {
            user.Avatar.Should().BeNull();
        }

        if (updateUser.DayOfBirth.HasValue)
        {
            user!.DayOfBirth!.Value.Date.Should().Be(updateUser.DayOfBirth.Value.Date);
        }
        else
        {
            user.DayOfBirth.Should().BeNull();
        }

        if (updateUser.CommuneId != null || updateUser.CommuneId != Ulid.Empty)
        {
            user.Address.Commune!.Id.Should().Be(updateUser.CommuneId!.Value);
        }
        else
        {
            user.Address.Commune.Should().BeNull();
        }

        user.UserRoles!.Select(x => x.RoleId).Should().Contain(updateUser.Roles);

        if (updateUser.UserClaims?.Count > 0)
        {
            user.UserClaims!.Select(x => new { x.ClaimType, x.ClaimValue })
                .Should()
                .IntersectWith(
                    updateUser.UserClaims.Select(x => new { x.ClaimType, x.ClaimValue })!
                );
        }
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        await testingFixture.SeedingRegionsAsync();
        await testingFixture.SeedingUserAsync();
        updateUserCommand = await testingFixture.CreateUserAsync();
    }
}
