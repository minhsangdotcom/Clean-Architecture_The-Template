using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Queries.Detail;

public class GetUserDetailMapping : Profile
{
    public GetUserDetailMapping()
    {
        CreateMap<User, GetUserDetailResponse>();
    }
}