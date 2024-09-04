using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Commands.Update;

public class UpdateUserMapping : Profile
{
    public UpdateUserMapping()
    {
        CreateMap<UpdateUser, User>();
        CreateMap<User, UpdateUserResponse>();
    }
}