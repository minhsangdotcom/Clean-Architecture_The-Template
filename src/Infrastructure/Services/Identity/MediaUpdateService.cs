using Application.Common.Interfaces.Services.Identity;
using Application.Common.Interfaces.Services.Storage;
using Contracts.Dtos.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Identity;

public class MediaUpdateService<T>(
    IStorageService storageService,
    ILogger<MediaUpdateService<T>> logger
) : IMediaUpdateService<T>
    where T : class
{
    private readonly string Directory = $"{typeof(T).Name}s";

    public async Task DeleteAvatarAsync(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var response = await storageService.DeleteAsync(key);

        if (!response.IsSuccess)
        {
            logger.LogInformation(
                "Remove object {key} fail with error: {error}",
                key,
                response.Error
            );
            return;
        }

        logger.LogInformation("Remove object {key} successfully.", key);
    }

    public string? GetKey(IFormFile? avatar)
    {
        if (avatar == null)
        {
            return null;
        }

        return $"{Directory}/{storageService.UniqueFileName(avatar.FileName)}";
    }

    public async Task<string?> UploadAvatarAsync(IFormFile? avatar, string? key)
    {
        if (avatar == null || string.IsNullOrEmpty(key))
        {
            return null;
        }

        StorageResponse response = await storageService.UploadAsync(avatar!.OpenReadStream(), key);

        if (!response.IsSuccess)
        {
            logger.LogInformation(
                "\nUpdate User has had error with file upload: {error}.\n",
                response.Error
            );
            return null;
        }

        logger.LogInformation(
            "\nUpdate avatar success full with the path: {path}.\n",
            response.FilePath
        );
        return key;
    }
}
