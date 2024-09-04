using Application.Common.Interfaces.Services;
using Contracts.Dtos.Responses;
using Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services;

public class AvatarUpdateService<T>(IAwsAmazonService awsAmazonService) : IAvatarUpdateService<T> where T : BaseEntity
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
            Console.WriteLine($"remove object {key} fail with error:{response.Error}");
            return;
        }

        Console.WriteLine($"remove object {key} successfully.");
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

        AwsResponse response = await awsAmazonService.UploadAsync(
               avatar!.OpenReadStream(),
               key
           );

        if (!response.IsSuccess)
        {
            Console.WriteLine($"\nUpdate User has had error with file upload: {response.Error}.\n");
            return null;
        }

        Console.WriteLine($"Update avatar success full with the path:{response.S3UploadedPath}.\n");

        return key;
    }
}