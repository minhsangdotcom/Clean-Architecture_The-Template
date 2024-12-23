using Application.Common.Exceptions;
using Application.Common.Interfaces.Services.Identity;
using Application.Features.Common.Mapping.Roles;
using Application.Features.Common.Projections.Roles;
using Application.Features.Roles.Commands.Update;
using AutoFixture;
using AutoMapper;
using Contracts.ApiWrapper;
using Contracts.Common.Messages;
using Domain.Aggregates.Roles;
using FluentAssertions;
using Moq;

namespace Application.UnitTest.Roles.Commands.Update;

public class UpdateRoleHandlerTest
{
    private readonly Mock<IRoleManagerService> roleManagerServiceMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly IMapper mapper;

    private readonly Fixture fixture = new();

    public UpdateRoleHandlerTest()
    {
        roleManagerServiceMock = new Mock<IRoleManagerService>();
        var mapperConfig = new MapperConfiguration(x =>
        {
            x.AddProfile<UpdateRoleMapping>();
            x.AddProfile<RoleMapping>();
        });

        mapper = mapperConfig.CreateMapper();
        mapperMock = new Mock<IMapper>();
    }

    [Fact]
    public async Task UpdateRole_WhenIdNotFound_ShouldReturnNotFoundException()
    {
        UpdateRole updatedRole = fixture
            .Build<UpdateRole>()
            .With(x => x.Name)
            .With(x => x.Description)
            .Without(x => x.RoleClaims)
            .Create();

        Ulid ulid = Ulid.NewUlid();
        UpdateRoleCommand updateRoleCommand = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, ulid.ToString())
            .With(x => x.Role, updatedRole)
            .Create();

        List<MessageResult> messageResults =
        [
            Messager.Create<Role>().Message(MessageType.Found).Negative().BuildMessage(),
        ];
        roleManagerServiceMock
            .Setup(x => x.GetByIdAsync(ulid))
            .Throws(new NotFoundException(messageResults));

        var handler = new UpdateRoleHandler(roleManagerServiceMock.Object, mapperMock.Object);
        Func<Task<UpdateRoleResponse>> updateRoleHandler = async () =>
            await handler.Handle(updateRoleCommand, CancellationToken.None);

        var result = await updateRoleHandler
            .Should()
            .ThrowAsync<NotFoundException>(becauseArgs: messageResults);
        ReasonTranslation error = result.And.Errors.First().Reasons.First();
        MessageResult messageResult = messageResults[0];
        error.Should().NotBeNull();

        error.Message.Should().Be(messageResult.Message);
        error.En.Should().Be(messageResult.En);
        error.Vi.Should().Be(messageResult.Vi);
    }

    [Fact]
    public async Task UpdateRole_WhenNoRoleClaims_ShouldUpdateRole()
    {
        UpdateRole updatedRole = fixture
            .Build<UpdateRole>()
            .With(x => x.Name)
            .With(x => x.Description)
            .OmitAutoProperties()
            .Create();

        Ulid ulid = Ulid.NewUlid();
        UpdateRoleCommand updateRoleCommand = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, ulid.ToString())
            .With(x => x.Role, updatedRole)
            .Create();

        Role currentRole = fixture
            .Build<Role>()
            .With(x => x.Id, ulid)
            .OmitAutoProperties()
            .Create();

        roleManagerServiceMock.Setup(x => x.GetByIdAsync(ulid)).ReturnsAsync(currentRole);
        mapper.Map(updatedRole, currentRole);
        mapperMock.Setup(x => x.Map(updatedRole, currentRole)).Returns(currentRole);
        UpdateRoleResponse updateRoleResponse = mapper.Map<UpdateRoleResponse>(currentRole);
        mapperMock.Setup(x => x.Map<UpdateRoleResponse>(currentRole)).Returns(updateRoleResponse);

        var handler = new UpdateRoleHandler(roleManagerServiceMock.Object, mapperMock.Object);
        var response = await handler.Handle(updateRoleCommand, CancellationToken.None);

        response.Id.Should().Be(updateRoleResponse.Id);
        response.Name.Should().Be(updateRoleResponse.Name);
    }

    [Fact]
    public async Task UpdateRole_WhenNoDescription_ShouldUpdateRole()
    {
        Ulid ulid = Ulid.NewUlid();
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        UpdateRole updatedRole = fixture
            .Build<UpdateRole>()
            .With(x => x.Name)
            .With(x => x.RoleClaims, roleClaims)
            .OmitAutoProperties()
            .Create();
        UpdateRoleCommand updateRoleCommand = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, ulid.ToString())
            .With(x => x.Role, updatedRole)
            .Create();

        var currentRoleClaims = fixture
            .Build<RoleClaim>()
            .With(x => x.Id, Ulid.NewUlid())
            .With(x => x.RoleId, ulid)
            .Without(x => x.Role)
            .Without(x => x.UserClaims)
            .CreateMany(2)
            .ToList();
        Role currentRole = fixture
            .Build<Role>()
            .With(x => x.Id, ulid)
            .With(x => x.RoleClaims, currentRoleClaims)
            .Without(x => x.UserRoles)
            .Create();

        roleManagerServiceMock.Setup(x => x.GetByIdAsync(ulid)).ReturnsAsync(currentRole);
        mapper.Map(updatedRole, currentRole);
        mapperMock.Setup(x => x.Map(updatedRole, currentRole)).Returns(currentRole);

        roleManagerServiceMock
            .Setup(x => x.UpdateRoleAsync(currentRole, It.IsAny<List<RoleClaim>>()))
            .ReturnsAsync(It.IsAny<Role>());

        UpdateRoleResponse updateRoleResponse = mapper.Map<UpdateRoleResponse>(currentRole);
        mapperMock.Setup(x => x.Map<UpdateRoleResponse>(currentRole)).Returns(updateRoleResponse);

        var handler = new UpdateRoleHandler(roleManagerServiceMock.Object, mapperMock.Object);
        var response = await handler.Handle(updateRoleCommand, CancellationToken.None);

        response.Id.Should().Be(updateRoleResponse.Id);
        response.Name.Should().Be(updateRoleResponse.Name);
        roleManagerServiceMock.Verify(
            x => x.UpdateRoleAsync(currentRole, It.IsAny<List<RoleClaim>>()),
            Times.Once
        );
        var expectedRoleClaims = updateRoleResponse.RoleClaims;
        response
            .RoleClaims.Should()
            .HaveCount(updateRoleResponse.RoleClaims!.Count())
            .And.ContainInOrder(expectedRoleClaims);
    }

    [Fact]
    public async Task UpdateRole_ShouldUpdateRole()
    {
        Ulid ulid = Ulid.NewUlid();
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        UpdateRole updatedRole = fixture
            .Build<UpdateRole>()
            .With(x => x.Name)
            .With(x => x.RoleClaims, roleClaims)
            .Create();
        UpdateRoleCommand updateRoleCommand = fixture
            .Build<UpdateRoleCommand>()
            .With(x => x.RoleId, ulid.ToString())
            .With(x => x.Role, updatedRole)
            .Create();

        var currentRoleClaims = fixture
            .Build<RoleClaim>()
            .With(x => x.Id, Ulid.NewUlid())
            .With(x => x.RoleId, ulid)
            .Without(x => x.Role)
            .Without(x => x.UserClaims)
            .CreateMany(2)
            .ToList();
        Role currentRole = fixture
            .Build<Role>()
            .With(x => x.Id, ulid)
            .With(x => x.RoleClaims, currentRoleClaims)
            .Without(x => x.UserRoles)
            .Create();

        roleManagerServiceMock.Setup(x => x.GetByIdAsync(ulid)).ReturnsAsync(currentRole);
        mapper.Map(updatedRole, currentRole);
        mapperMock.Setup(x => x.Map(updatedRole, currentRole)).Returns(currentRole);

        roleManagerServiceMock
            .Setup(x => x.UpdateRoleAsync(currentRole, It.IsAny<List<RoleClaim>>()))
            .ReturnsAsync(It.IsAny<Role>());

        UpdateRoleResponse updateRoleResponse = mapper.Map<UpdateRoleResponse>(currentRole);
        mapperMock.Setup(x => x.Map<UpdateRoleResponse>(currentRole)).Returns(updateRoleResponse);

        var handler = new UpdateRoleHandler(roleManagerServiceMock.Object, mapperMock.Object);
        var response = await handler.Handle(updateRoleCommand, CancellationToken.None);

        response.Id.Should().Be(updateRoleResponse.Id);
        response.Name.Should().Be(updateRoleResponse.Name);
        roleManagerServiceMock.Verify(
            x => x.UpdateRoleAsync(currentRole, It.IsAny<List<RoleClaim>>()),
            Times.Once
        );
        var expectedRoleClaims = updateRoleResponse.RoleClaims;
        response
            .RoleClaims.Should()
            .HaveCount(updateRoleResponse.RoleClaims!.Count())
            .And.ContainInOrder(expectedRoleClaims);
    }
}
