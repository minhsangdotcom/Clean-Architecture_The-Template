using Application.SubcutaneousTests.Extensions;
using Application.UseCases.Projections.Users;
using Application.UseCases.Users.Commands.Create;
using AutoFixture;
using Domain.Aggregates.Users;
using FluentAssertions;
using Infrastructure.Constants;
using Microsoft.AspNetCore.Http;

namespace Application.SubcutaneousTests.Users.Create;

[Collection(nameof(TestingCollectionFixture))]
public class CreateUserHandlerTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();

    private Ulid roleId;

    private CreateUserCommand command = new();

    [Fact]
    private async Task CreateUser_WhenNoCustomClaim_ShouldCreateSuccess()
    {
        command.UserClaims = null;

        CreateUserResponse response = await testingFixture.SendAsync(command);
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, command);
    }

    [Fact]
    private async Task CreateUser_WhenNoAvatar_ShouldCreateSuccess()
    {
        command.Avatar = null;
        CreateUserResponse response = await testingFixture.SendAsync(command);
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, command);
    }

    [Fact]
    private async Task CreateUser_WhenNoGender_ShouldCreateSuccess()
    {
        command.Gender = null;
        CreateUserResponse response = await testingFixture.SendAsync(command);
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, command);
    }

    [Fact]
    private async Task CreateUser_WhenNoDayOfBirth_ShouldCreateSuccess()
    {
        command.DayOfBirth = null;
        CreateUserResponse response = await testingFixture.SendAsync(command);
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, command);
    }

    [Fact]
    private async Task CreateUser_ShouldCreateSuccess()
    {
        CreateUserResponse response = await testingFixture.SendAsync(command);
        User? user = await testingFixture.FindUserByIdAsync(response.Id);

        AssertUser(user, command);
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

        IFormFile file = FileHelper.GenerateIFormfile(
            Path.Combine(Directory.GetCurrentDirectory(), "Files", "avatar_cute_2.jpg")
        );
        command = fixture
            .Build<CreateUserCommand>()
            .With(x => x.ProvinceId, Ulid.Parse("01JAZDXCWY3Z9K3XS0AYZ733NF"))
            .With(x => x.DistrictId, Ulid.Parse("01JAZDXDGSP0J0XF10836TR3QY"))
            .With(x => x.CommuneId, Ulid.Parse("01JAZDXEAV440AJHTVEV0QTAV5"))
            .With(x => x.Avatar, file)
            .With(
                x => x.UserClaims,
                Credential
                    .MANAGER_CLAIMS.Select(x => new UserClaimModel()
                    {
                        ClaimType = x.Key,
                        ClaimValue = x.Value,
                    })
                    .ToList()
            )
            .With(x => x.Roles, [roleId])
            .With(x => x.Email, "super.admin@gmail.com")
            .With(x => x.PhoneNumber, "0925123123")
            .With(x => x.Username, "super.admin")
            .Create();
    }

    private void AssertUser(User? user, CreateUserCommand createUserCommand)
    {
        user.Should().NotBeNull();
        user!.FirstName.Should().Be(createUserCommand.FirstName);
        user!.LastName.Should().Be(createUserCommand.LastName);
        user!.Email.Should().Be(createUserCommand.Email);
        user!.PhoneNumber.Should().Be(createUserCommand.PhoneNumber);
        user!.Address!.Province!.Id.Should().Be(createUserCommand.ProvinceId);
        user!.Address!.District!.Id.Should().Be(createUserCommand.DistrictId);
        user!.Username!.Should().Be(createUserCommand.Username);
        BCrypt.Net.BCrypt.Verify(createUserCommand.Password, user.Password).Should().BeTrue();

        if (createUserCommand.Avatar != null)
        {
            user.Avatar.Should().NotBeNull();
        }
        else
        {
            user.Avatar.Should().BeNull();
        }

        if (createUserCommand.DayOfBirth.HasValue)
        {
            user!.DayOfBirth!.Value.Date.Should().Be(createUserCommand.DayOfBirth.Value.Date);
        }
        else
        {
            user.DayOfBirth.Should().BeNull();
        }

        user!.Gender.Should().Be(createUserCommand.Gender);
        user!.Status.Should().Be(createUserCommand.Status);

        if (createUserCommand.CommuneId != null || createUserCommand.CommuneId != Ulid.Empty)
        {
            user.Address.Commune!.Id.Should().Be(createUserCommand.CommuneId!.Value);
        }
        else
        {
            user.Address.Commune.Should().BeNull();
        }

        user.UserRoles.Should().ContainSingle(x => x.RoleId == roleId);

        if (createUserCommand.UserClaims?.Count > 0)
        {
            user.UserClaims!.Select(x => new { x.ClaimType, x.ClaimValue })
                .Should()
                .IntersectWith(
                    createUserCommand.UserClaims.Select(x => new { x.ClaimType, x.ClaimValue })!
                );
        }
    }
}
