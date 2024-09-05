using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Commands.Login;

public class LoginUserMapping : Profile
{
    public LoginUserMapping()
    {
        CreateMap<User, UserProjection>();
    }
}