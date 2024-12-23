using Application.UseCases.Projections.Users;
using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Queries.List;

public class ListUserMapping : Profile
{
    public ListUserMapping()
    {
        CreateMap<User, ListUserResponse>().IncludeBase<User, UserProjection>();
        // .ForMember(
        //     dest => dest.Age,
        //     opt =>
        //         opt.MapFrom(src =>
        //             src.DayOfBirth == null
        //                 ? 0
        //                 : DateTimeOffset.UtcNow.Year - src.DayOfBirth.Value.Year
        //         )
        // );
    }
}
