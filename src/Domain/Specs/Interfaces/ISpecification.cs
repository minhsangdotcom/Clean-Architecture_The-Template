using System.Linq.Expressions;
using Domain.Common;
using Domain.Specs.Models;

namespace Domain.Specs.Interfaces;

public interface ISpecification<T> where T : BaseEntity
{
    SpecificationBuilder<T> Query { get;}

    Expression<Func<T, bool>> Criteria { get; }

    List<IncludeInfo> Includes { get; }

    bool IsNoTracking { get; }

    bool IsSplitQuery { get; }
}