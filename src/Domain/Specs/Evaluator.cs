using Domain.Specs.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Domain.Specs;

public class SpecificationEvaluator<T> where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        IQueryable<T> query = inputQuery;

        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }

        if (specification.IsNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (specification.Includes.Count > 0)
        {
            query = query.Include(specification.Includes);
        }

        if (specification.IsSplitQuery)
        {
            query = query.AsSplitQuery();
        }

        return query;
    }
}