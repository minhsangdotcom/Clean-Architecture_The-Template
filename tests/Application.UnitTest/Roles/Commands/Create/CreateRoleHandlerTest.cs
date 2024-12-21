using Application.Common.Interfaces.Services.Identity;
using Application.UseCases.Projections.Roles;
using Application.UseCases.Roles.Commands.Create;
using AutoFixture;
using AutoMapper;
using Domain.Aggregates.Roles;
using FluentAssertions;
using Moq;

namespace Application.UnitTest.Roles.Commands.Create;

public class CreateRoleHandlerTest
{
    private readonly Mock<IRoleManagerService> roleManagerServiceMock;
    private readonly Mock<IMapper> mapperMock;
    private readonly IMapper mapper;

    private readonly Fixture fixture = new();

    public CreateRoleHandlerTest()
    {
        roleManagerServiceMock = new Mock<IRoleManagerService>();
        var mapperConfig = new MapperConfiguration(x =>
        {
            x.AddProfile<CreateRoleMapping>();
        });

        mapper = mapperConfig.CreateMapper();
        mapperMock = new Mock<IMapper>();
    }

    [Fact]
    public async Task CreateRole_WhenNoRoleClaims_ShouldCreateRole()
    {
        var command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name)
            .With(x => x.Description)
            .Without(x => x.RoleClaims)
            .Create();

        Role mappedRole = mapper.Map<Role>(command);
        CreateRoleResponse createdRole = mapper.Map<CreateRoleResponse>(mappedRole);

        mapperMock.Setup(x => x.Map<Role>(command)).Returns(mappedRole);
        roleManagerServiceMock.Setup(x => x.CreateRoleAsync(mappedRole)).ReturnsAsync(mappedRole);
        mapperMock.Setup(x => x.Map<CreateRoleResponse>(mappedRole)).Returns(createdRole);

        var handler = new CreateRoleHandler(roleManagerServiceMock.Object, mapperMock.Object);
        var response = await handler.Handle(command, CancellationToken.None);

        response.Should().BeOfType<CreateRoleResponse>();
        response.Name.Should().Be(createdRole.Name);
        response.Id.Should().Be(createdRole.Id);

        mapperMock.Verify(mapper => mapper.Map<Role>(command), Times.Once);
        roleManagerServiceMock.Verify(service => service.CreateRoleAsync(mappedRole), Times.Once);
        mapperMock.Verify(mapper => mapper.Map<CreateRoleResponse>(mappedRole), Times.Once);
    }

    [Fact]
    public async Task CreateRole_WhenNoDescription_ShouldCreateRole()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        var command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name)
            .With(x => x.RoleClaims, roleClaims)
            .Create();

        Role mappedRole = mapper.Map<Role>(command);
        CreateRoleResponse createdRole = mapper.Map<CreateRoleResponse>(mappedRole);

        mapperMock.Setup(x => x.Map<Role>(command)).Returns(mappedRole);
        roleManagerServiceMock.Setup(x => x.CreateRoleAsync(mappedRole)).ReturnsAsync(mappedRole);
        mapperMock.Setup(x => x.Map<CreateRoleResponse>(mappedRole)).Returns(createdRole);

        var handler = new CreateRoleHandler(roleManagerServiceMock.Object, mapperMock.Object);
        var response = await handler.Handle(command, CancellationToken.None);

        response.Should().BeOfType<CreateRoleResponse>();
        response.Name.Should().Be(createdRole.Name);
        response.Id.Should().Be(createdRole.Id);

        mapperMock.Verify(mapper => mapper.Map<Role>(command), Times.Once);
        roleManagerServiceMock.Verify(service => service.CreateRoleAsync(mappedRole), Times.Once);
        mapperMock.Verify(mapper => mapper.Map<CreateRoleResponse>(mappedRole), Times.Once);
    }

    [Fact]
    public async Task CreateRole_ShouldCreateRole()
    {
        var roleClaims = fixture.Build<RoleClaimModel>().Without(x => x.Id).CreateMany(2).ToList();
        var command = fixture
            .Build<CreateRoleCommand>()
            .With(x => x.Name)
            .With(x => x.RoleClaims, roleClaims)
            .With(x => x.Description, fixture.Create<string>())
            .Create();

        Role mappedRole = mapper.Map<Role>(command);
        CreateRoleResponse createdRole = mapper.Map<CreateRoleResponse>(mappedRole);

        mapperMock.Setup(x => x.Map<Role>(command)).Returns(mappedRole);
        roleManagerServiceMock.Setup(x => x.CreateRoleAsync(mappedRole)).ReturnsAsync(mappedRole);
        mapperMock.Setup(x => x.Map<CreateRoleResponse>(mappedRole)).Returns(createdRole);

        var handler = new CreateRoleHandler(roleManagerServiceMock.Object, mapperMock.Object);
        var response = await handler.Handle(command, CancellationToken.None);

        response.Should().BeOfType<CreateRoleResponse>();
        response.Name.Should().Be(createdRole.Name);
        response.Id.Should().Be(createdRole.Id);

        mapperMock.Verify(mapper => mapper.Map<Role>(command), Times.Once);
        roleManagerServiceMock.Verify(service => service.CreateRoleAsync(mappedRole), Times.Once);
        mapperMock.Verify(mapper => mapper.Map<CreateRoleResponse>(mappedRole), Times.Once);
    }
}
