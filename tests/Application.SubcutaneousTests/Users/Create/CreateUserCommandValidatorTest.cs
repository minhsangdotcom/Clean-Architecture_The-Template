using System.Net;
using Application.Common.Exceptions;
using Application.Features.Users.Commands.Create;
using Application.SubcutaneousTests.Extensions;
using AutoFixture;
using Contracts.ApiWrapper;
using Domain.Aggregates.Users.Enums;
using FluentAssertions;

namespace Application.SubcutaneousTests.Users.Create;

[Collection(nameof(TestingCollectionFixture))]
public class CreateUserCommandValidatorTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private Ulid roleId;
    private readonly Fixture fixture = new();
    private CreateUserCommand command = new();

    [Fact]
    public async Task CreateUser_WhenNoFirstName_ShouldReturnValidationException()
    {
        command.FirstName = null;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenInvalidLengthOfFirstName_ShouldReturnValidationException()
    {
        command.FirstName = new string(fixture.CreateMany<char>(257).ToArray());
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNoLastName_ShouldReturnValidationException()
    {
        command.LastName = null;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenInvalidLengthOfLastName_ShouldReturnValidationException()
    {
        command.LastName = new string(fixture.CreateMany<char>(257).ToArray());
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenDuplicatedEmail_ShouldReturnValidationException()
    {
        command.Email = "admin.admin@admin.com.vn";

        var response = await testingFixture.MakeRequestAsync(
            "users",
            HttpMethod.Post,
            command,
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
    public async Task CreateUser_WhenInvalidEmailFormat_ShouldReturnValidationException(
        string email
    )
    {
        command.Email = email;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData("1234567890123456")]
    [InlineData("+12345")]
    public async Task CreateUser_WhenInvalidPhoneNumberFormat_ShouldReturnValidationException(
        string phoneNumber
    )
    {
        command.PhoneNumber = phoneNumber;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNoProvince_ShouldReturnValidationException()
    {
        command.ProvinceId = Ulid.Empty;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNotFoundProvince_ShouldReturnValidationException()
    {
        command.ProvinceId = Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NN");
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNoDistrict_ShouldReturnValidationException()
    {
        command.DistrictId = Ulid.Empty;
        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNotFoundDistrict_ShouldReturnValidationException()
    {
        command.DistrictId = Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QQ");

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNotFoundCommune_ShouldReturnValidationException()
    {
        command.CommuneId = Ulid.Parse("01JAZDXEAV440AJHTVEV0QTAVV");

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNoUsername_ShouldReturnValidationException()
    {
        command.Username = null;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData("admin-super")]
    [InlineData("admin@super")]
    [InlineData("admin123!")]
    public async Task CreateUser_WhenInvalidUsername_ShouldReturnValidationException(
        string username
    )
    {
        command.Username = username;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenDuplicatedUsername_ShouldReturnValidationException()
    {
        command.Username = "admin";

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNoPassword_ShouldReturnValidationException()
    {
        command.Password = null;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData("admin@123")]
    [InlineData("adminadmin")]
    [InlineData("admin")]
    public async Task CreateUser_WhenInvalidPassword_ShouldReturnValidationException(
        string password
    )
    {
        command.Password = password;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public async Task CreateUser_WhenInvalidGender_ShouldReturnValidationException(int gender)
    {
        command.Gender = (Gender)gender;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNoStatus_ShouldReturnValidationException()
    {
        command.Status = 0;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(3)]
    public async Task CreateUser_WhenInvalidStatus_ShouldReturnValidationException(int status)
    {
        command.Status = (UserStatus)status;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNoRoles_ShouldReturnValidationException()
    {
        command.Roles = null;

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenDuplicatedRole_ShouldReturnValidationException()
    {
        command.Roles!.Add(roleId);

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNotFoundRole_ShouldReturnValidationException()
    {
        command.Roles!.Add(Ulid.NewUlid());

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
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
        var response = await testingFixture.CreateRoleAsync("adminTest");
        roleId = response.Id;
        await testingFixture.SeedingUserAsync();
        command = fixture
            .Build<CreateUserCommand>()
            .With(x => x.ProvinceId, Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NF"))
            .With(x => x.DistrictId, Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QY"))
            .With(x => x.CommuneId, Ulid.Parse("01JAZDXEAV440AJHTVEV0QTAV5"))
            .Without(x => x.Avatar)
            .Without(x => x.UserClaims)
            .With(x => x.Roles, [roleId])
            .With(x => x.Email, "super.admin@gmail.com")
            .With(x => x.PhoneNumber, "0925123123")
            .With(x => x.Username, "super.admin")
            .Create();
    }
}
