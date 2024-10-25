using System.Linq.Expressions;
using Application.Common.Interfaces.Registers;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Domain.Aggregates.Regions;

namespace Application.Common.Interfaces.Services;

public interface IRegionService : IScope
{
    Task<IEnumerable<Province>> Provinces();
    Task<PaginationResponse<T>> Districts<T>(QueryParamRequest request);
    Task<PaginationResponse<T>> Communes<T>(QueryParamRequest request);

    Task<Province?> FindProvinceByCode(string code);
    Task<District?> FindDistrictyCode(string code);
    Task<Commune?> FindCommuneByCode(string code);

    Task<IEnumerable<T>> CreateRangeAsync<T>(IEnumerable<T> data) where T : class;
    
    Task<bool> AnyAsync<T>(Expression<Func<T, bool>>? expression = null) where T : class;
}
