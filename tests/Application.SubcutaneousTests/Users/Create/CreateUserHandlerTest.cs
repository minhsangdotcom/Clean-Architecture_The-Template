using Application.Features.Common.Projections.Users;
using Application.Features.Users.Commands.Create;
using Application.SubcutaneousTests.Extensions;
using AutoFixture;
using Domain.Aggregates.Roles;
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
        UserAddress address = await testingFixture.SeedingRegionsAsync();
        Role role = await testingFixture.CreateAdminRoleAsync();
        roleId = role.Id;

        IFormFile file = FileHelper.GenerateIFormfile(
            Path.Combine(Directory.GetCurrentDirectory(), "Files", "avatar_cute_2.jpg")
        );
        command = fixture
            .Build<CreateUserCommand>()
            .With(x => x.ProvinceId, address.ProvinceId)
            .With(x => x.DistrictId, address.DistrictId)
            .With(x => x.CommuneId, address.CommuneId)
            .With(x => x.Avatar, file)
            .With(
                x => x.UserClaims,
                [
                    .. Credential.MANAGER_CLAIMS.Select(x => new UserClaimModel()
                    {
                        ClaimType = x.Key,
                        ClaimValue = x.Value,
                    }),
                ]
            )
            .With(x => x.Roles, [roleId])
            .With(x => x.Email, "admin@gmail.com")
            .With(x => x.PhoneNumber, "0123456789")
            .With(x => x.Username, "admin.super")
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
