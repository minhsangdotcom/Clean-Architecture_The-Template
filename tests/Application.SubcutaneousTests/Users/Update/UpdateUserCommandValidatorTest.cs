using System.Net;
using Application.Common.Exceptions;
using Application.Features.Users.Commands.Update;
using Application.SubcutaneousTests.Extensions;
using AutoFixture;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Application.SubcutaneousTests.Users.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateUserCommandValidatorTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();
    private UpdateUserCommand updateUserCommand = new();

    [Fact]
    public async Task UpdateUser_WhenNoFirstName_ShouldReturnValidationException()
    {
        updateUserCommand.User!.FirstName = null;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenInvalidLengthOfFirstName_ShouldReturnValidationException()
    {
        updateUserCommand.User!.FirstName = new string(fixture.CreateMany<char>(257).ToArray());
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoLastName_ShouldReturnValidationException()
    {
        updateUserCommand.User!.LastName = null;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenInvalidLengthOfLastName_ShouldReturnValidationException()
    {
        updateUserCommand.User!.LastName = new string(fixture.CreateMany<char>(257).ToArray());
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenDuplicatedEmail_ShouldReturnValidationException()
    {
        User user = await testingFixture.CreateAdminUserAsync();
        updateUserCommand.User!.Email = user.Email;

        var response = await testingFixture.MakeRequestAsync(
            "users",
            HttpMethod.Post,
            updateUserCommand.User!,
            "multipart/form-data"
        );

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ErrorResponse? errorResponse = await response.ToResponse<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        List<BadRequestError> badRequestErrors = [.. errorResponse!.Errors!];

        BadRequestError firstElement = badRequestErrors[0];

        firstElement.PropertyName.Should().Be("Email");
        List<ReasonTranslation> reasons = [.. firstElement.Reasons];

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
        updateUserCommand.User!.Email = email;

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
        updateUserCommand.User!.PhoneNumber = phoneNumber;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoProvince_ShouldReturnValidationException()
    {
        updateUserCommand.User!.ProvinceId = Ulid.Empty;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNotFoundProvince_ShouldReturnValidationException()
    {
        updateUserCommand.User!.ProvinceId = Ulid.NewUlid();
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoDistrict_ShouldReturnValidationException()
    {
        updateUserCommand.User!.DistrictId = Ulid.Empty;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNotFoundDistrict_ShouldReturnValidationException()
    {
        updateUserCommand.User!.DistrictId = Ulid.NewUlid();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNotFoundCommune_ShouldReturnValidationException()
    {
        updateUserCommand.User!.CommuneId = Ulid.NewUlid();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoRoles_ShouldReturnValidationException()
    {
        updateUserCommand.User!.Roles = null!;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoClaimType_ShouldReturnValidationException()
    {
        updateUserCommand.User!.UserClaims!.ForEach(x => x.ClaimType = null);

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateUser_WhenNoClaimValue_ShouldReturnValidationException()
    {
        updateUserCommand.User!.UserClaims!.ForEach(x => x.ClaimValue = null);

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(updateUserCommand))
            .Should()
            .ThrowAsync<ValidationException>();
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
