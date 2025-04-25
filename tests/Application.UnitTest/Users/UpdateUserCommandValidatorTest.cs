using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Projections.Users;
using Application.Features.Users.Commands.Update;
using AutoFixture;
using Domain.Aggregates.Users;
using FluentValidation;
using FluentValidation.TestHelper;
using Moq;
using SharedKernel.Common.Messages;

namespace Application.UnitTest.Users;

public class UpdateUserCommandValidatorTest
{
    private readonly UserUpdateRequest userUpdate;
    private readonly Fixture fixture = new();

    private readonly UpdateUserCommandValidator validator;
    private readonly InlineValidator<UserUpdateRequest> mockValidator = [];

    public UpdateUserCommandValidatorTest()
    {
        Mock<IUserManagerService> mockUserManagerService = new();
        Mock<IHttpContextAccessorService> mockHttpContextAccessorService = new();
        validator = new(mockUserManagerService.Object, mockHttpContextAccessorService.Object);
        userUpdate = fixture
            .Build<UserUpdateRequest>()
            .With(x => x.ProvinceId, Ulid.Parse("01JRQHWS3RQR1N0J84EV1DQXR1"))
            .With(x => x.DistrictId, Ulid.Parse("01JRQHWSNPR3Z8Z20GBSB22CSJ"))
            .With(x => x.CommuneId, Ulid.Parse("01JRQHWTCHN5WBZ12WC08AZCZ8"))
            .Without(x => x.Avatar)
            .With(
                x => x.UserClaims,
                [new UserClaimModel() { ClaimType = "test", ClaimValue = "test.value" }]
            )
            .With(x => x.Roles, [Ulid.Parse("01JS72XZJ6NFKFVWA9QM03RY5G")])
            .With(x => x.Email, "admin@gmail.com")
            .With(x => x.PhoneNumber, "0123456789")
            .Create();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WhenFirstNameNullOrEmpty_ShouldReturnNullFailure(string? firstName)
    {
        //arrage
        userUpdate!.FirstName = firstName;

        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(x => x.FirstName)
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Fact]
    public async Task Validate_WhenInvalidLengthOfFirstName_ShouldReturnMaximumLengthFailure()
    {
        userUpdate!.FirstName = new string([.. fixture.CreateMany<char>(257)]);

        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(x => x.FirstName)
            .Message(MessageType.MaximumLength)
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WhenLastNameNullOrEmpty_ShouldReturnNullFailure(string? lastName)
    {
        userUpdate!.LastName = lastName;
        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(x => x.LastName)
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.LastName)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Fact]
    public async Task Validate_WhenInvalidLengthOfLastName_ShouldReturnMaximumLengthFailure()
    {
        userUpdate!.LastName = new string([.. fixture.CreateMany<char>(257)]);

        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(x => x.LastName)
            .Message(MessageType.MaximumLength)
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.LastName)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_WhenEmailNullOrEmpty_ShouldReturnNullFailure(string? email)
    {
        userUpdate!.Email = email;

        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(x => x.Email)
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Theory]
    [InlineData("admin@gmail")]
    [InlineData("admingmail.com")]
    [InlineData("@gmail.com")]
    public async Task CreateUser_WhenEmailInvalidFormat_ShouldReturnInvalidFailure(string email)
    {
        userUpdate!.Email = email;

        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(x => x.Email)
            .Message(MessageType.Valid)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Fact]
    public async Task Validate_WhenEmailDuplicated_ShouldReturnExistFailure()
    {
        const string existedEmail = "admin@gmail.com";
        userUpdate!.Email = existedEmail;
        var expectedState = Messager
            .Create<User>()
            .Property(x => x.Email)
            .Message(MessageType.Existence)
            .Build();

        mockValidator
            .RuleFor(x => x.Email)
            .MustAsync(
                (email, cancellationToken) =>
                    IsEmailAvailableAsync(email!, existedEmail, cancellationToken)
            )
            .When(_ => true)
            .WithState(x => expectedState);

        //act
        var result = await mockValidator.TestValidateAsync(userUpdate);

        //assert
        result
            .ShouldHaveValidationErrorFor(x => x.Email)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validate_WhenPhoneNumberNullOrEmpty_ShouldReturNullFailure(string phoneNumber)
    {
        userUpdate.PhoneNumber = phoneNumber;

        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(x => x.PhoneNumber)
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Theory]
    [InlineData("123456")]
    [InlineData("++12345678")]
    public async Task Validate_WhenPhoneNumberInvalidFormat_ShouldReturnInvalidFailure(
        string phoneNumber
    )
    {
        userUpdate.PhoneNumber = phoneNumber;

        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(x => x.PhoneNumber)
            .Message(MessageType.Valid)
            .Negative()
            .Build();

        result
            .ShouldHaveValidationErrorFor(x => x.PhoneNumber)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Fact]
    public async Task Validate_WhenProvinceEmpty_ShouldReturnNullFailure()
    {
        userUpdate!.ProvinceId = Ulid.Empty;

        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(nameof(UserUpdateRequest.ProvinceId))
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.ProvinceId)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Fact]
    public async Task Validate_WhenDistrictEmpty_ShouldReturnNullFailure()
    {
        userUpdate!.DistrictId = Ulid.Empty;
        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(nameof(UserUpdateRequest.DistrictId))
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.DistrictId)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_WhenStreetNullOrEmpty_ShouldReturnNullFailure(string? street)
    {
        userUpdate!.Street = street;

        //act
        var result = await validator.TestValidateAsync(userUpdate);

        //assert
        var expectedState = Messager
            .Create<User>()
            .Property(nameof(UserUpdateRequest.Street))
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.Street)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Fact]
    public async Task Validate_WhenRolesNull_ShouldReturnNullFailure()
    {
        userUpdate!.Roles = null;

        //act
        var result = await validator.TestValidateAsync(userUpdate);
        //assert
        var expectedState = Messager
            .Create<UserUpdateRequest>(nameof(User))
            .Property(x => x.Roles!)
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result
            .ShouldHaveValidationErrorFor(x => x.Roles)
            .WithCustomState(expectedState, new MessageResultComparer())
            .Only();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_WhenClaimTypeisNullOrEmpty_ShouldReturnNullFailure(string type)
    {
        userUpdate!.UserClaims!.ForEach(x => x.ClaimType = type);

        //act
        var result = await validator.TestValidateAsync(userUpdate);
        //assert
        var expectedState = Messager
            .Create<UserClaim>(nameof(User.UserClaims))
            .Property(x => x.ClaimType!)
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result.ShouldHaveValidationErrorFor(
            $"{nameof(User.UserClaims)}[0].{nameof(UserClaimModel.ClaimType)}"
        );
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Validate_WhenClaimValueNullOrEmpty_ShouldReturnNullFailure(string? value)
    {
        userUpdate!.UserClaims!.ForEach(x => x.ClaimValue = value);

        //act
        var result = await validator.TestValidateAsync(userUpdate);
        //assert
        var expectedState = Messager
            .Create<UserClaim>(nameof(User.UserClaims))
            .Property(x => x.ClaimValue!)
            .Message(MessageType.Null)
            .Negative()
            .Build();
        result.ShouldHaveValidationErrorFor(
            $"{nameof(User.UserClaims)}[0].{nameof(UserClaimModel.ClaimValue)}"
        );
    }

    private static async Task<bool> IsEmailAvailableAsync(
        string email,
        string existedEmail,
        CancellationToken cancellationToken = default
    )
    {
        return await Task.Run(() => email != existedEmail, cancellationToken);
    }
}
