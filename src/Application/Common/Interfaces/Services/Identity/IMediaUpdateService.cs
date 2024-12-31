using Application.Common.Interfaces.Registers;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Interfaces.Services.Identity;

public interface IMediaUpdateService<T> : ISingleton
    where T : class
{
    string? GetKey(IFormFile? avatar);

    Task<string?> UploadAvatarAsync(IFormFile? avatar, string? key);

    Task DeleteAvatarAsync(string? key);
}
