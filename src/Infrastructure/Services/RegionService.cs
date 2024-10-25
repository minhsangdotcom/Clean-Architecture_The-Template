using System.Linq.Expressions;
using Application.Common.Interfaces.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts.Dtos.Models;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Contracts.Extensions.QueryExtensions;
using Domain.Aggregates.Regions;
using Domain.Common;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class RegionService(IDbContext dbContext, IMapper mapper) : IRegionService
{
    private readonly DbSet<Province> provinces = dbContext.Set<Province>();
    private readonly DbSet<District> districts = dbContext.Set<District>();
    private readonly DbSet<Commune> communes = dbContext.Set<Commune>();

    public Task<bool> AnyAsync<T>(Expression<Func<T, bool>>? expression = null)
        where T : class => dbContext.Set<T>().AnyAsync(expression ?? (x => true));

    public async Task<PaginationResponse<T>> Communes<T>(QueryParamRequest request)
    {
        string defaultSort = string.IsNullOrWhiteSpace(request.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : request.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";
        Search? search = request.Search;

        return await communes
            .ProjectTo<T>(mapper.ConfigurationProvider)
            .Filter(request.DynamicFilter)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToPagedListAsync(request.Page, request.PageSize);
    }

    public async Task<IEnumerable<T>> CreateRangeAsync<T>(IEnumerable<T> data) where T : class
    {
        await dbContext.Set<T>().AddRangeAsync(data);
        await dbContext.SaveChangesAsync();
        return data;
    }

    public async Task<PaginationResponse<T>> Districts<T>(QueryParamRequest request)
    {
        string defaultSort = string.IsNullOrWhiteSpace(request.Sort)
            ? $"{nameof(BaseEntity.CreatedAt)}{OrderTerm.DELIMITER}{OrderTerm.DESC}"
            : request.Sort.Trim();
        string uniqueSort = $"{defaultSort},{nameof(BaseEntity.Id)}";
        Search? search = request.Search;

        return await districts
            .ProjectTo<T>(mapper.ConfigurationProvider)
            .Filter(request.DynamicFilter)
            .Search(search?.Keyword, search?.Targets)
            .Sort(uniqueSort)
            .ToPagedListAsync(request.Page, request.PageSize);
    }

    public async Task<Commune?> FindCommuneByCode(string code) =>
        await communes.Where(x => x.Code == code).FirstOrDefaultAsync();

    public async Task<District?> FindDistrictyCode(string code) =>
        await districts.Where(x => x.Code == code).FirstOrDefaultAsync();

    public async Task<Province?> FindProvinceByCode(string code) =>
        await provinces.Where(x => x.Code == code).FirstOrDefaultAsync();

    public async Task<IEnumerable<Province>> Provinces() => await provinces.ToListAsync();
}
