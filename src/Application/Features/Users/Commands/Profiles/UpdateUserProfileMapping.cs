using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.Features.Users.Commands.Profiles;

public class UpdateUserProfileMapping : Profile
{
    public UpdateUserProfileMapping()
    {
        CreateMap<UpdateUserProfileCommand, User>();
        CreateMap<User, UpdateUserProfileResponse>();
    }
}
