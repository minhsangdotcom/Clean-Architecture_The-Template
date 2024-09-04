using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services;

public interface IAvatarUpdateService<T> where T : BaseEntity
{
    string? GetKey(IFormFile? avatar);

    Task<string?> UploadAvatarAsync(IFormFile? avatar, string? key);

    Task DeleteAvatarAsync(string? key);
}