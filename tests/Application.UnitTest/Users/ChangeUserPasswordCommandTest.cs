using Application.Features.Users.Commands.ChangePassword;
using Domain.Aggregates.Users;
using FluentValidation.TestHelper;
using SharedKernel.Common.Messages;

namespace Application.UnitTest.Users;

public class ChangeUserPasswordCommandTest
{
    private readonly ChangeUserPasswordCommand command =
        new() { OldPassword = "Admin@123", NewPassword = "Admin@456" };

    private readonly ChangeUserPasswordCommandValidator validator = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_WhenOldPassowrdIsNullOrEmpty_ShouldReturnNullFailure(
        string oldPassword
    )
    {
        //arrage
        command.OldPassword = oldPassword;

        //act
        var result = await validator.TestValidateAsync(command);

        //assert
        var expectedMessage = Messager
            .Create<ChangeUserPasswordCommand>(nameof(User))
            .Property(x => x.OldPassword!)
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.OldPassword)
            .WithCustomState(expectedMessage, new MessageResultComparer())
            .Only();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_WhenNewPassowrdNullOrEmpty_ShouldReturnNullFailure(string password)
    {
        //arrage
        command.NewPassword = password;

        //act
        var result = await validator.TestValidateAsync(command);

        //assert
        var expectedMessage = Messager
            .Create<ChangeUserPasswordCommand>(nameof(User))
            .Property(x => x.NewPassword!)
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithCustomState(expectedMessage, new MessageResultComparer())
            .Only();
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("admin@123")]
    [InlineData("admin0123")]
    public async Task Validate_WhenNewPassowrdNotStrong_ShouldReturnNullFailure(string password)
    {
        //arrage
        command.NewPassword = password;

        //act
        var result = await validator.TestValidateAsync(command);

        //assert
        var expectedMessage = Messager
            .Create<ChangeUserPasswordCommand>(nameof(User))
            .Property(x => x.NewPassword!)
            .Message(MessageType.Strong)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.NewPassword)
            .WithCustomState(expectedMessage, new MessageResultComparer())
            .Only();
    }
}
