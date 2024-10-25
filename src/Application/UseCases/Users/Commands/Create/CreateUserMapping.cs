using Application.UseCases.Projections.Roles;
using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Roles;
using Domain.Aggregates.Users;
using Domain.Aggregates.Users.Enums;

namespace Application.UseCases.Users.Commands.Create;

public class CreateUserMapping : Profile
{
    public CreateUserMapping()
    {
        CreateMap<CreateUserCommand, User>()
            .AfterMap((src, dest) =>
            {
                dest.SetPassword(HashPassword(src.Password));
            });

        CreateMap<UserClaimModel, UserClaimType>()
            .AfterMap((src, dest, context) =>
            {
                if (Enum.TryParse(context.Items[nameof(UserClaimType.Type)]?.ToString(), out KindaUserClaimType type))
                {
                    dest.Type = type;
                }
            });

        CreateMap<User, CreateUserResponse>();
    }
}