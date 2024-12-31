using Application.Features.Common.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.Features.Users.Commands.Login;

public class LoginUserMapping : Profile
{
    public LoginUserMapping()
    {
        CreateMap<User, UserProjection>();
    }
}
