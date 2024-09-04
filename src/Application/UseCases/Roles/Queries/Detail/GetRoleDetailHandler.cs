using Application.Common.Exceptions;
using Application.Common.Interfaces.Services;
using AutoMapper;
using Domain.Aggregates.Users;
using Mediator;

namespace Application.UseCases.Roles.Queries.Detail;

public class GetRoleDetailHandler(IRoleManagerService roleManagerService, IMapper mapper) : IRequestHandler<GetRoleDetailQuery, RoleDetailResponse>
{
    public async ValueTask<RoleDetailResponse> Handle(GetRoleDetailQuery query, CancellationToken cancellationToken) =>
        mapper.Map<RoleDetailResponse>(
            await roleManagerService.FindByIdAsync(query.Id) ?? throw new BadRequestException($"{nameof(Role).ToUpper()}_NOTFOUND")
    );
}