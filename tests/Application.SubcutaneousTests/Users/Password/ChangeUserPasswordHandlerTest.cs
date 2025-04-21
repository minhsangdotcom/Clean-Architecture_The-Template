using Application.Features.Users.Commands.ChangePassword;
using Application.SubcutaneousTests.Extensions;
using Domain.Aggregates.Users;
using SharedKernel.Common.Messages;
using Shouldly;

namespace Application.SubcutaneousTests.Users.Password;

[Collection(nameof(TestingCollectionFixture))]
public class ChangeUserPasswordHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private Ulid id;

    [Fact]
    public async Task ChangePassword_WhenUserNotFound_ShouldReturnNotFoundResult()
    {
        //arrage
        TestingFixture.RemoveUserId();
        //act
        var result = await testingFixture.SendAsync(new ChangeUserPasswordCommand());
        //assert
        var expectedMessage = Messager.Create<User>().Message(MessageType.Found).Negative().Build();
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error?.ErrorMessage.ShouldBe(expectedMessage, new MessageResultComparer());
    }

    [Fact]
    public async Task ChangePassword_WhenOldPasswordInCorrect_ShouldReturnInCorrectPasswordResult()
    {
        //act
        var result = await testingFixture.SendAsync(
            new ChangeUserPasswordCommand() { OldPassword = "Admin@423", NewPassword = "Admin@456" }
        );
        //assert
        var expectedMessage = Messager
            .Create<ChangeUserPasswordCommand>(nameof(User))
            .Property(x => x.OldPassword!)
            .Message(MessageType.Correct)
            .Negative()
            .Build();
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error?.ErrorMessage.ShouldBe(expectedMessage, new MessageResultComparer());
    }

    [Fact]
    public async Task ChangePassword_ShouldSuccess()
    {
        string newPassowrd = "Admin@456";
        //act
        var result = await testingFixture.SendAsync(
            new ChangeUserPasswordCommand()
            {
                OldPassword = TestingFixture.DEFAULT_USER_PASSWORD,
                NewPassword = newPassowrd,
            }
        );
        //assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        var user = await testingFixture.FindUserByIdAsync(id);
        user.ShouldNotBeNull();
        BCrypt.Net.BCrypt.Verify(newPassowrd, user.Password).ShouldBeTrue();
    }

    public async Task DisposeAsync() => await Task.CompletedTask;

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
        var regions = await testingFixture.SeedingRegionsAsync();
        var user = await testingFixture.CreateNormalUserAsync(
            new UserAddress(regions.ProvinceId, regions.DistrictId, regions.CommuneId)
        );
        id = user.Id;
    }
}
