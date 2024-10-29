using Application.Common.Interfaces.Services.Aws;
using Application.Common.Interfaces.Services.Identity;
using Contracts.Dtos.Responses;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace Infrastructure.Services.Identity;

public class MediaUpdateService<T>(IAmazonS3Service awsAmazonService, ILogger logger)
    : IMediaUpdateService<T>
    where T : class
{
    private readonly string Directory = $"{typeof(T).Name}s";

    public async Task DeleteAvatarAsync(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            return;
        }

        var response = await awsAmazonService.DeleteAsync(key);

        if (!response.IsSuccess)
        {
            logger.Information("Remove object {key} fail with error: {error}", key, response.Error);
            return;
        }

        logger.Information("Remove object {key} successfully.", key);
    }

    public string? GetKey(IFormFile? avatar)
    {
        if (avatar == null)
        {
            return null;
        }

        return $"{Directory}/{awsAmazonService.UniqueFileName(avatar.FileName)}";
    }

    public async Task<string?> UploadAvatarAsync(IFormFile? avatar, string? key)
    {
        if (avatar == null || string.IsNullOrEmpty(key))
        {
            return null;
        }

        AwsResponse response = await awsAmazonService.UploadAsync(avatar!.OpenReadStream(), key);

        if (!response.IsSuccess)
        {
            logger.Information(
                "\nUpdate User has had error with file upload: {error}.\n",
                response.Error
            );
            return null;
        }

        logger.Information(
            "\nUpdate avatar success full with the path: {path}.\n",
            response.S3UploadedPath
        );
        return key;
    }
}
