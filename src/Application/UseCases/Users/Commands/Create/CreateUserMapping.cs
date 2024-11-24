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

        CreateMap<UserClaimModel, UserClaim>()
            .AfterMap(
                (src, dest, context) =>
                {
                    if (
                        Enum.TryParse(
                            context.Items[nameof(UserClaim.Type)]?.ToString(),
                            out KindaUserClaimType type
                        )
                    )
                    {
                        dest.Type = type;
                    }

                    if (
                        Ulid.TryParse(
                            context.Items[nameof(UserClaim.UserId)]?.ToString(),
                            out Ulid id
                        )
                    )
                    {
                        dest.UserId = id;
                    }

                    if (src.Id == null || src.Id == Ulid.Empty)
                    {
                        dest.Id = Ulid.NewUlid();
                    }
                }
            );

        CreateMap<User, CreateUserResponse>();
    }
}
