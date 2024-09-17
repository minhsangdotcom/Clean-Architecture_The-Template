
using Domain.Common;
namespace Domain.Specs.Interfaces;

//builder design pattern
public interface ISpecificationBuilder<T> where T : class
{
    Specification<T>? Spec { get; }
}

// include
public interface IIncludableSpecificationBuilder<T, TProperty> : ISpecificationBuilder<T>
    where T : class
{
}