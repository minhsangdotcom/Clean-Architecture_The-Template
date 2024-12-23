using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;

namespace Application.UseCases.Users.Commands.Create;

public class CreateUserMapping : Profile
{
    public CreateUserMapping()
    {
        CreateMap<CreateUserCommand, User>()
            .ForMember(dest => dest.UserClaims, opt => opt.Ignore())
            .ForMember(dest => dest.UserRoles, opt => opt.Ignore())
            .AfterMap(
                (src, dest) =>
                {
                    dest.SetPassword(HashPassword(src.Password));
                }
            );

        CreateMap<User, CreateUserResponse>().IncludeBase<User, UserDetailProjection>();
    }
}
