namespace Application.Common.Interfaces.UnitOfWorks;

public interface IRepository<T>
    : IRepositoryAsync<T>,
        IRepositorySync<T>,
        IRepositorySpecification<T>
    where T : class { }
