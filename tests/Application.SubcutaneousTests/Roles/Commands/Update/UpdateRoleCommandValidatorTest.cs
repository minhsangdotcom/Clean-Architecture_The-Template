using System.Net;
using System.Net.Http.Json;
using System.Text;
using Application.Common.Exceptions;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Roles.Commands.Create;
using Application.UseCases.Roles.Commands.Update;
using AutoFixture;
using Contracts.ApiWrapper;
using Contracts.Extensions;
using FluentAssertions;

namespace Application.SubcutaneousTests.Roles.Commands.Update;

[Collection(nameof(TestingCollectionFixture))]
public class UpdateRoleCommandValidatorTest(TestingFixture testingFixture) : IAsyncLifetime
{
    private readonly Fixture fixture = new();

    private UpdateRoleCommand updateRoleCommand = new();

    [Fact]
    public async Task UpdateRole_WhenNoName_ShouldReturnValidationException()
    {
        UpdateRole updateRole = fixture
            .Build<UpdateRole>()
            .Without(x => x.Name)
            .With(x => x.Description, updateRoleCommand.Role.Description)
            .With(x => x.RoleClaims, updateRoleCommand.Role.RoleClaims)
            .Create();

        UpdateRoleCommand editedRole = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, updateRoleCommand.RoleId)
            .With(x => x.Role, updateRole)
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(editedRole))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateRole_WhenInvalidLengthName_ShouldReturnValidationException()
    {
        UpdateRole updateRole = fixture
            .Build<UpdateRole>()
            .With(x => x.Name, new string(fixture.CreateMany<char>(258).ToArray()))
            .With(x => x.Description, updateRoleCommand.Role.Description)
            .With(x => x.RoleClaims, updateRoleCommand.Role.RoleClaims)
            .Create();

        UpdateRoleCommand editedRole = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, updateRoleCommand.RoleId)
            .With(x => x.Role, updateRole)
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(editedRole))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task UpdateRole_WhenDuplicatedName_ShouldReturnDuplicatedMessage()
    {
        _ = await CreateRoleAsync("userTest");
        UpdateRole updateRole = fixture
            .Build<UpdateRole>()
            .With(x => x.Name, "userTest")
            .With(x => x.Description, updateRoleCommand.Role.Description)
            .With(x => x.RoleClaims, updateRoleCommand.Role.RoleClaims)
            .Create();

        UpdateRoleCommand editedRole = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, updateRoleCommand.RoleId)
            .With(x => x.Role, updateRole)
            .Create();

        StringContent payload =
            new(
                SerializerExtension.Serialize(editedRole.Role).StringJson,
                Encoding.UTF8,
                "application/json"
            );
        var response = await testingFixture
            .CreateClient()
            .PutAsync($"http://localhost:8080/api/roles/{editedRole.RoleId}", payload);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        ErrorResponse? errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        errorResponse.Should().NotBeNull();
        List<BadRequestError> badRequestErrors = [.. errorResponse!.Errors!];

        BadRequestError firstElement = badRequestErrors[0];

        firstElement.PropertyName.Should().Be("Role.Name");
        List<ReasonTranslation> reasons = [.. firstElement.Reasons];

        reasons[0].Message.Should().Be("role_name_existence");
    }

    [Fact]
    public async Task CreateRole_WhenInvalidLengthDescription_ShouldReturnValidationException()
    {
        UpdateRole updateRole = fixture
            .Build<UpdateRole>()
            .With(x => x.Name, updateRoleCommand.Role.Name)
            .With(x => x.Description, new string(fixture.CreateMany<char>(1001).ToArray()))
            .With(x => x.RoleClaims, updateRoleCommand.Role.RoleClaims)
            .Create();

        UpdateRoleCommand editedRole = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, updateRoleCommand.RoleId)
            .With(x => x.Role, updateRole)
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(editedRole))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimTypeOfRoleClaim_ShouldReturnValidationException()
    {
        UpdateRole updateRole = fixture
            .Build<UpdateRole>()
            .With(x => x.Name, updateRoleCommand.Role.Name)
            .With(x => x.Description, updateRoleCommand.Role.Description)
            .With(
                x => x.RoleClaims,
                updateRoleCommand
                    .Role.RoleClaims!.Select(x => new RoleClaimModel()
                    {
                        Id = x.Id,
                        ClaimValue = x.ClaimValue,
                    })
                    .ToList()
            )
            .Create();

        UpdateRoleCommand editedRole = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, updateRoleCommand.RoleId)
            .With(x => x.Role, updateRole)
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(editedRole))
            .Should()
            .ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CreateRole_WhenMissingClaimValueOfRoleClaim_ShouldReturnValidationException()
    {
         UpdateRole updateRole = fixture
            .Build<UpdateRole>()
            .With(x => x.Name, updateRoleCommand.Role.Name)
            .With(x => x.Description, updateRoleCommand.Role.Description)
            .With(
                x => x.RoleClaims,
                updateRoleCommand
                    .Role.RoleClaims!.Select(x => new RoleClaimModel()
                    {
                        Id = x.Id,
                        ClaimType = x.ClaimType,
                    })
                    .ToList()
            )
            .Create();

        UpdateRoleCommand editedRole = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, updateRoleCommand.RoleId)
            .With(x => x.Role, updateRole)
            .Create();

        await FluentActions
            .Invoking(() => testingFixture.SendAsync(editedRole))
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

        CreateRoleResponse createRoleResponse = await CreateRoleAsync("managerTest");
        updateRoleCommand = new()
        {
            RoleId = createRoleResponse.Id.ToString(),
            Role = new UpdateRole()
            {
                Name = createRoleResponse.Name,
                Description = createRoleResponse.Description,
                RoleClaims = createRoleResponse
                    .RoleClaims!.Select(x => new RoleClaimModel()
                    {
                        ClaimType = x.ClaimType,
                        ClaimValue = x.ClaimValue,
                        Id = x.Id,
                    })
                    .ToList(),
            },
        };
    }

    private async Task<CreateRoleResponse> CreateRoleAsync(string roleName)
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        CreateRoleCommand createRoleCommand = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name, roleName)
            .With(x => x.Description)
            .With(x => x.RoleClaims, roleClaims)
            .Create();

        return await testingFixture.SendAsync(createRoleCommand);
    }
}
