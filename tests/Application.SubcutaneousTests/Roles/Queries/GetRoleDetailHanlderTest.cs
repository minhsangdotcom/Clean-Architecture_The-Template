using Application.Features.Roles.Queries.Detail;
using Application.SubcutaneousTests.Extensions;
using Domain.Aggregates.Roles;
using SharedKernel.Common.Messages;
using Shouldly;

namespace Application.SubcutaneousTests.Roles.Queries;

[Collection(nameof(TestingCollectionFixture))]
public class GetRoleDetailHanlderTest(TestingFixture testingFixture) : IAsyncLifetime
{
    [Fact]
    public async Task GetRole_WhenIdNotFound_ShouldReturnNotFoundResult()
    {
        //arrage
        Ulid Id = Ulid.Empty;
        //act
        var result = await testingFixture.SendAsync(new GetRoleDetailQuery(Id));
        //assert
        var expectedMessage = Messager.Create<Role>().Message(MessageType.Found).Negative().Build();
        result.IsSuccess.ShouldBeFalse();
        result.Error.ShouldNotBeNull();
        result.Error?.ErrorMessage.ShouldBe(expectedMessage, new MessageResultComparer());
    }

    [Fact]
    public async Task GetRole_ShouldSuccess()
    {
        //arrage
        var role = await testingFixture.CreateNormalRoleAsync();
        //act
        var result = await testingFixture.SendAsync(new GetRoleDetailQuery(role.Id));
        //assert
        result.IsSuccess.ShouldBeTrue();
        result.Error.ShouldBeNull();

        result.Value?.Id.ShouldBe(role.Id);
        result.Value?.Name.ShouldBe(role.Name);
    }

    public async Task DisposeAsync() => await Task.CompletedTask;

    public async Task InitializeAsync()
    {
        await testingFixture.ResetAsync();
    }
}
