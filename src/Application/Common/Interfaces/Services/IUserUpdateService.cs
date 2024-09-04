using Domain.Aggregates.Users;

namespace Application.Common.Interfaces.Services;

public interface IUserUpdateService
{
    Task<User> UpdateAsync(long userId, object request, Func<User, Task>? additionWork = null);
}