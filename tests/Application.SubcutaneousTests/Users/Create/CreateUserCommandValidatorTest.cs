using Application.Common.Exceptions;
using Application.UseCases.Users.Commands.Create;
using AutoFixture;
using FluentAssertions;

namespace Application.SubcutaneousTests.Users.Create;

[Collection(nameof(TestingCollectionFixture))]
public class CreateUserCommandValidatorTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();

    private Ulid roleId;

    [Fact]
    public async Task CreateUser_WhenNoFirstName_ShouldReturnValidationException()
    {
        var command = fixture
            .Build<CreateUserCommand>()
            .With(x => x.ProvinceId, Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NF"))
            .With(x => x.DistrictId, Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QY"))
            .With(x => x.CommuneId, Ulid.Parse("01JAZDXEAV440AJHTVEV0QTAV5"))
            .Without(x => x.Avatar)
            .Without(x => x.FirstName)
            .Without(x => x.UserClaims)
            .With(x => x.Roles, [roleId])
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenInvalidLengthOfFirstName_ShouldReturnValidationException()
    {
        var command = fixture
            .Build<CreateUserCommand>()
            .With(x => x.ProvinceId, Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NF"))
            .With(x => x.DistrictId, Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QY"))
            .With(x => x.CommuneId, Ulid.Parse("01JAZDXEAV440AJHTVEV0QTAV5"))
            .Without(x => x.Avatar)
            .Without(x => x.UserClaims)
            .With(x => x.Roles, [roleId])
            .With(x => x.FirstName, new string(fixture.CreateMany<char>(257).ToArray()))
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenNoLastName_ShouldReturnValidationException()
    {
        var command = fixture
            .Build<CreateUserCommand>()
            .With(x => x.ProvinceId, Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NF"))
            .With(x => x.DistrictId, Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QY"))
            .With(x => x.CommuneId, Ulid.Parse("01JAZDXEAV440AJHTVEV0QTAV5"))
            .Without(x => x.Avatar)
            .Without(x => x.UserClaims)
            .With(x => x.Roles, [roleId])
            .Without(x => x.LastName)
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(command))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateUser_WhenInvalidLengthOfLastName_ShouldReturnValidationException()
    {
        var command = fixture
            .Build<CreateUserCommand>()
            .With(x => x.ProvinceId, Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NF"))
            .With(x => x.DistrictId, Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QY"))
            .With(x => x.CommuneId, Ulid.Parse("01JAZDXEAV440AJHTVEV0QTAV5"))
            .Without(x => x.Avatar)
            .Without(x => x.UserClaims)
            .With(x => x.Roles, [roleId])
            .With(x => x.LastName, new string(fixture.CreateMany<char>(257).ToArray()))
            .Create();

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
        var response = await testingFixture.CreateRoleAsync("admin", fixture);
        roleId = Ulid.Parse(response.RoleId);
    }
}
