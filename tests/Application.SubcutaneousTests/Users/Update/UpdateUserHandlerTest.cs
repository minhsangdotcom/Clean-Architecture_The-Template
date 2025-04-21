using Application.Features.Users.Commands.Update;
using Application.SubcutaneousTests.Extensions;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users;
using Microsoft.AspNetCore.Http;
using SharedKernel.Common.Messages;
using Shouldly;

namespace Application.SubcutaneousTests.Users.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateUserHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private UpdateUserCommand updateUserCommand = new();

    [Fact]
    private async Task UpdateUser_WhenProvinceNotFound_ShouldReturnNotFoundResult()
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

        result.Error.ShouldNotBeNull();
        result.Error.Status.ShouldBe(404);
        result.Error.ErrorMessage.ShouldBe(expectedMessage, new MessageResultComparer());
    }

    [Fact]
    private async Task UpdateUser_WhenDistrictNotFound_ShouldReturnNotFoundResult()
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

        result.Error.ShouldNotBeNull();
        result.Error.Status.ShouldBe(404);
        result.Error.ErrorMessage.ShouldBe(expectedMessage, new MessageResultComparer());
    }

    [Fact]
    private async Task UpdateUser_WhenCommuneNotFound_ShouldReturnNotFoundResult()
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

        result.Error.ShouldNotBeNull();
        result.Error.Status.ShouldBe(404);
        result.Error.ErrorMessage.ShouldBe(expectedMessage, new MessageResultComparer());
    }

    [Fact]
    private async Task UpdateUser_WhenIdNotfound_ShouldReturnNotFoundResult()
    {
        updateUserCommand.UserId = Ulid.NewUlid().ToString();
        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);
        var expectedMessage = Messager
            .Create<User>()
            .Message(MessageType.Found)
            .Negative()
            .BuildMessage();

        result.Error.ShouldNotBeNull();
        result.Error.Status.ShouldBe(404);
        result.Error.ErrorMessage.ShouldBe(expectedMessage, new MessageResultComparer());
    }

    [Fact]
    private async Task UpdateProfile_ShouldUpdateSuccess()
    {
        //arrage
        var updateData = updateUserCommand.UpdateData;
        updateData.DayOfBirth = null;
        updateData.Avatar = null;
        updateData.UserClaims = null;
        //act
        Result<UpdateUserResponse> result = await testingFixture.SendAsync(updateUserCommand);

        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        var response = result.Value!;
        var user = await testingFixture.FindUserByIdAsync(response.Id);
        user.ShouldNotBeNull();

        user!.ShouldSatisfyAllConditions(
            () => user.Id.ShouldBe(response.Id),
            () => user.FirstName.ShouldBe(response.FirstName),
            () => user.LastName.ShouldBe(response.LastName),
            () => user.Username.ShouldBe(response.Username),
            () => user.Email.ShouldBe(response.Email),
            () => user.PhoneNumber.ShouldBe(response.PhoneNumber),
            () => user.Gender.ShouldBe(response.Gender),
            () => user.Address?.ToString().ShouldBe(response.Address),
            () => user.Status.ShouldBe(response.Status),
            () =>
                user
                    .UserRoles?.All(x => updateData.Roles?.Any(p => p == x.RoleId) == true)
                    .ShouldBeTrue(),
            () =>
                updateData
                    .UserClaims?.All(x =>
                        user.UserClaims?.Any(p =>
                            p.ClaimType == x.ClaimType && p.ClaimValue == x.ClaimType
                        ) == true
                    )
                    .ShouldBeTrue()
        );
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
