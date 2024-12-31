using Domain.Common.Specs;
using Domain.Common.Specs.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Domain.Common.Specs;

public class SpecificationEvaluator<T>
    where T : class
{
    public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
    {
        IQueryable<T> query = inputQuery;
        return Evaluate(query, specification);
    }

    public static string SpecStringQuery(ISpecification<T> specification)
    {
        IQueryable<T> query = Enumerable.Empty<T>().AsQueryable();
        query = Evaluate(query, specification);
        return query.Expression.ToString();
    }

    private static IQueryable<T> Evaluate(IQueryable<T> query, ISpecification<T> specification)
    {
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
