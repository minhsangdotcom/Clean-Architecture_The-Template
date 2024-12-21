using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Commands.Update;

public class UpdateUserMapping : Profile
{
    public UpdateUserMapping()
    {
        CreateMap<UpdateUser, User>()
            .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore());
        CreateMap<User, UpdateUserResponse>();
    }
}
