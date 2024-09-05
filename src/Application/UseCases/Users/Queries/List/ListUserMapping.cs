using AutoMapper;
using Domain.Aggregates.Users;

namespace Application.UseCases.Users.Queries.List;

public class ListUserMapping : Profile
{
    public ListUserMapping()
    {
        CreateMap<User, ListUserResponse>();
    }
}