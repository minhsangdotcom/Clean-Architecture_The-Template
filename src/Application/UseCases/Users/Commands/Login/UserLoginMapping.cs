using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Commands.Login;

public class UserLoginMapping : Profile
{
    public UserLoginMapping()
    {
        CreateMap<User, UserProjection>();
    }
}
