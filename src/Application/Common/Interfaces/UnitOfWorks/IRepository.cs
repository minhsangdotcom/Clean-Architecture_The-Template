namespace Application.Common.Interfaces.UnitOfWorks;

public interface IRepository<T>
    : IRepositoryAsync<T>,
        IMemoryRepository<T>,
        ISpecificationRepository<T>,
        IStaticPredicateRepository<T>
    where T : class { }
