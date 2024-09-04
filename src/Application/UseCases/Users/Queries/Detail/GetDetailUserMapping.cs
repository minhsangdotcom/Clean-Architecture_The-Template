using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Queries.Detail;

public class GetDetailUserMapping : Profile
{
    public GetDetailUserMapping()
    {
        CreateMap<User, GetDetailUserResponse>();
    }
}