using Application.Common.Interfaces.Registers;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;

namespace Application.Common.Interfaces.Services.Aws;

public interface IStorageService : ISingleton
{
    Task<StorageResponse> UploadAsync(Stream stream, string key);

    Task<StorageResponse> UploadAsync(string path, string key);

    Task<StorageResponse> UploadMultiplePartAsync(MultiplePartUploadRequest request);

    Task<StorageResponse> DeleteAsync(string key);

    string? GetFullpath(string? key);

    string UniqueFileName(string fileName);

    string? GetPublicUrl();
}
