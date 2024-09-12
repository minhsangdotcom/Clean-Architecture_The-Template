using Application.Common.Interfaces.Registers;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;

namespace Application.Common.Interfaces.Services.Aws;

public interface IAwsAmazonService : ISingleton
{
    Task<AwsResponse> UploadAsync(Stream stream, string key);

    Task<AwsResponse> UploadAsync(string path, string key);

    Task<AwsResponse> UploadMultiplePartAsync(AwsRequest request);

    Task<AwsResponse> DeleteAsync(string key);

    string? GetFullpath(string? key);

    string UniqueFileName(string fileName);

    string? GetPublicUrl();
}