using System.Net;
using Application.Common.Exceptions;
using Application.Features.Users.Commands.Update;
using Application.SubcutaneousTests.Extensions;
using AutoFixture;
using Contracts.ApiWrapper;
using FluentAssertions;

namespace Application.SubcutaneousTests.Users.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateUserCommandValidatorTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();
    private UpdateUserCommand updateUserCommand = new();

    [Fact]
    public async Task UpdateUser_WhenNoFirstName_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.FirstName = null;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenInvalidLengthOfFirstName_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.FirstName = new string(fixture.CreateMany<char>(257).ToArray());
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoLastName_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.LastName = null;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenInvalidLengthOfLastName_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.LastName = new string(fixture.CreateMany<char>(257).ToArray());
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData("admin.admin@admin.com.vn")]
    public async Task UpdateUser_WhenDuplicatedEmail_ShouldReturnValidationException(string name)
    {
        updateUserCommand.UpdateData.Email = name;

        var response = await testingFixture.MakeRequestAsync(
            "users",
            HttpMethod.Post,
            updateUserCommand.UpdateData,
            "multipart/form-data"
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ErrorResponse? errorResponse = await response.ToResponse<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        List<InvalidParam> badRequestErrors = [.. errorResponse!.Errors!];

        InvalidParam firstElement = badRequestErrors[0];

        firstElement.PropertyName.Should().Be("Email");
        List<ErrorReason> reasons = [.. firstElement.Reasons];

        reasons[0].Message.Should().Be("user_email_existence");
    }

    [Theory]
    [InlineData("admin@gmail")]
    [InlineData("admingmail.com")]
    [InlineData("@gmail.com")]
    public async Task UpdateUser_WhenInvalidEmailFormat_ShouldReturnValidationException(
        string email
    )
    {
        updateUserCommand.UpdateData.Email = email;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData("1234567890123456")]
    [InlineData("+12345")]
    public async Task UpdateUser_WhenInvalidPhoneNumberFormat_ShouldReturnValidationException(
        string phoneNumber
    )
    {
        updateUserCommand.UpdateData.PhoneNumber = phoneNumber;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoProvince_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.ProvinceId = Ulid.Empty;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNotFoundProvince_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.ProvinceId = Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NN");
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoDistrict_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.DistrictId = Ulid.Empty;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNotFoundDistrict_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.DistrictId = Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QQ");

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNotFoundCommune_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.CommuneId = Ulid.Parse("01JAZDXEAV440AJHTVEV0QTAVV");

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoRoles_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.Roles = null!;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoClaimType_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.UserClaims!.ForEach(x => x.ClaimType = null);

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoClaimValue_ShouldReturnValidationException()
    {
        updateUserCommand.UpdateData.UserClaims!.ForEach(x => x.ClaimValue = null);

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
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
        await testingFixture.SeedingRegionsAsync();
        await testingFixture.SeedingUserAsync();
        updateUserCommand = await testingFixture.CreateUserAsync();
    }
}
