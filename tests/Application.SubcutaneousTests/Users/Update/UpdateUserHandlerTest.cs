using Application.Features.Users.Commands.Update;
using Application.SubcutaneousTests.Extensions;
using AutoFixture;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;

namespace Application.SubcutaneousTests.Users.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateUserHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();
    private UpdateUserCommand updateUserCommand = new();

    [Fact]
    private async Task CreateUser_WhenProvinceNotFound_ShouldReturnNotFoundResult()
    {
        updateUserCommand.UpdateData.ProvinceId = Ulid.NewUlid();
        //act
        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);

        //assert
        var expectedMessage = Messager
            .Create<User>()
            .Property(nameof(UserUpdateRequest.ProvinceId))
            .Message(MessageType.Existence)
            .Negative()
            .Build();

        result.Error.Should().NotBeNull();
        result.Error.Status.Should().Be(404);
        result.Error.ErrorMessage.Should().BeEquivalentTo(expectedMessage);
    }

    [Fact]
    private async Task CreateUser_WhenDistrictNotFound_ShouldReturnNotFoundResult()
    {
        updateUserCommand.UpdateData.DistrictId = Ulid.NewUlid();
        //act
        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);

        //assert
        var expectedMessage = Messager
            .Create<User>()
            .Property(nameof(UserUpdateRequest.DistrictId))
            .Message(MessageType.Existence)
            .Negative()
            .Build();

        result.Error.Should().NotBeNull();
        result.Error.Status.Should().Be(404);
        result.Error.ErrorMessage.Should().BeEquivalentTo(expectedMessage);
    }

    [Fact]
    private async Task CreateUser_WhenCommuneNotFound_ShouldReturnNotFoundResult()
    {
        updateUserCommand.UpdateData.CommuneId = Ulid.NewUlid();
        //act
        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);

        //assert
        var expectedMessage = Messager
            .Create<User>()
            .Property(nameof(UserUpdateRequest.CommuneId))
            .Message(MessageType.Existence)
            .Negative()
            .Build();

        result.Error.Should().NotBeNull();
        result.Error.Status.Should().Be(404);
        result.Error.ErrorMessage.Should().BeEquivalentTo(expectedMessage);
    }

    [Fact]
    private async Task UpdateUser_WhenIdNotfound_ShouldReturnNotFoundResult()
    {
        updateUserCommand.UserId = Ulid.NewUlid().ToString();
        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);
        var expectMessage = Messager
            .Create<User>()
            .Message(MessageType.Found)
            .Negative()
            .BuildMessage();

        result.Error.Should().NotBeNull();
        result.Error.Status.Should().Be(404);
        result.Error.ErrorMessage.Should().BeEquivalentTo(expectMessage);
    }

    [Fact]
    private async Task UpdateUser_WhenNoCustomClaim_ShouldUpdateSuccess()
    {
        updateUserCommand.UpdateData!.UserClaims = null;

        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);
        UpdateUserResponse response = result.Value!;
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, updateUserCommand);
    }

    [Fact]
    private async Task UpdateUser_WhenNoAvatar_ShouldUpdateSuccess()
    {
        updateUserCommand.UpdateData!.Avatar = null;
        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);
        UpdateUserResponse response = result.Value!;
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, updateUserCommand);
    }

    [Fact]
    private async Task UpdateUser_WhenNoDayOfBirth_ShouldUpdateSuccess()
    {
        updateUserCommand.UpdateData!.DayOfBirth = null;
        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);
        UpdateUserResponse response = result.Value!;
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, updateUserCommand);
    }

    [Fact]
    private async Task UpdateUser_ShouldUpdateSuccess()
    {
        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);
        UpdateUserResponse response = result.Value!;
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, updateUserCommand);
    }

    private static void AssertUser(User? user, UpdateUserCommand updateUserCommand)
    {
        UserUpdateRequest UserUpdateRequest = updateUserCommand.UpdateData!;
        user.Should().NotBeNull();
        user!.FirstName.Should().Be(UserUpdateRequest.FirstName);
        user!.LastName.Should().Be(UserUpdateRequest.LastName);
        user!.Email.Should().Be(UserUpdateRequest.Email);
        user!.PhoneNumber.Should().Be(UserUpdateRequest.PhoneNumber);
        user!.Address!.ProvinceId.Should().Be(UserUpdateRequest.ProvinceId);
        user!.Address!.DistrictId.Should().Be(UserUpdateRequest.DistrictId);

        if (UserUpdateRequest.Avatar != null)
        {
            user.Avatar.Should().NotBeNull();
        }
        else
        {
            user.Avatar.Should().BeNull();
        }

        if (UserUpdateRequest.DayOfBirth.HasValue)
        {
            user!.DayOfBirth!.Value.Date.Should().Be(UserUpdateRequest.DayOfBirth.Value.Date);
        }
        else
        {
            user.DayOfBirth.Should().BeNull();
        }

        if (UserUpdateRequest.CommuneId != null || UserUpdateRequest.CommuneId != Ulid.Empty)
        {
            user.Address.CommuneId.Should().Be(UserUpdateRequest.CommuneId!.Value);
        }
        else
        {
            user.Address.Commune.Should().BeNull();
        }

        user.UserRoles!.Select(x => x.RoleId).Should().Contain(UserUpdateRequest.Roles);

        if (UserUpdateRequest.UserClaims?.Count > 0)
        {
            user.UserClaims!.Select(x => new { x.ClaimType, x.ClaimValue })
                .Should()
                .IntersectWith(
                    UserUpdateRequest.UserClaims.Select(x => new { x.ClaimType, x.ClaimValue })!
                );
        }
    }

    public async Task DisposeAsync() => await Task.CompletedTask;

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        UserAddress address = await testingFixture.SeedingRegionsAsync();

        IFormFile file = FileHelper.GenerateIFormfile(
            Path.Combine(Directory.GetCurrentDirectory(), "Files", "avatar_cute_2.jpg")
        );

        updateUserCommand = UserMappingExtension.ToUpdateUserCommand(
            await testingFixture.CreateManagerUserAsync(address, file)
        );
    }
}
