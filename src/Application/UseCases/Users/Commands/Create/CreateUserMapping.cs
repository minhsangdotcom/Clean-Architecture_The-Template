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
                        Enum.TryParse(
                            context.Items[nameof(UserClaim.UserId)]?.ToString(),
                            out Ulid id
                        )
                    )
                    {
                        dest.UserId = id;
                    }
                }
            );

        CreateMap<User, CreateUserResponse>();
    }
}
