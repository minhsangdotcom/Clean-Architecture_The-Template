using Amazon.S3;
using Amazon.S3.Model;
using Application.Common.Interfaces.Services.Aws;
using Contracts.Dtos.Requests;
using Contracts.Dtos.Responses;
using Microsoft.Extensions.Options;
using SharedKernel.Extensions;

namespace Infrastructure.Services.Aws;

public class AwsAmazonService(IAmazonS3 amazonS3, IOptions<S3AwsSettings> awsSetting)
    : IStorageService
{
    private readonly S3AwsSettings s3AwsSettings = awsSetting.Value;

    public async Task<StorageResponse> UploadAsync(Stream stream, string key) =>
        await UploadAsync(
            new PutObjectRequest
            {
                BucketName = s3AwsSettings.BucketName,
                Key = key,
                InputStream = stream,
            }
        );

    public async Task<StorageResponse> UploadAsync(string path, string key) =>
        await UploadAsync(
            new PutObjectRequest
            {
                BucketName = s3AwsSettings.BucketName,
                Key = key,
                FilePath = path,
            }
        );

    private async Task<StorageResponse> UploadAsync(PutObjectRequest request)
    {
        StorageResponse response = new();
        try
        {
            PutObjectResponse res = await amazonS3.PutObjectAsync(request);
            response.FilePath = GeneratePreSignedURL(request.Key);
            response.Key = request.Key;
        }
        catch (AmazonS3Exception ex)
        {
            response.Error = ex.Message;
        }
        catch (Exception ex)
        {
            response.Error = ex.Message;
        }

        return response;
    }

    public async Task<StorageResponse> UploadMultiplePartAsync(MultiplePartUploadRequest request)
    {
        List<UploadPartResponse> uploadResponses = [];

        InitiateMultipartUploadRequest initiateRequest =
            new() { BucketName = s3AwsSettings.BucketName, Key = request.Key };

        InitiateMultipartUploadResponse initResponse = await amazonS3.InitiateMultipartUploadAsync(
            initiateRequest
        );

        StorageResponse storageResponse = new();
        try
        {
            long filePosition = 0;
            long partSize = request.PartSize;

            for (int i = 1; filePosition < request.ContentLength; i++)
            {
                partSize = Math.Min(partSize, request.ContentLength - filePosition);

                UploadPartRequest uploadRequest =
                    new()
                    {
                        BucketName = s3AwsSettings.BucketName,
                        Key = request.Key,
                        UploadId = initResponse.UploadId,
                        PartNumber = i,
                        PartSize = request.PartSize,
                        FilePosition = filePosition,
                    };

                if (string.IsNullOrWhiteSpace(request.Path))
                {
                    uploadRequest.InputStream = request.InputStream;
                }
                else
                {
                    uploadRequest.FilePath = request.Path;
                }

                uploadResponses.Add(await amazonS3.UploadPartAsync(uploadRequest));

                filePosition += request.PartSize;
            }

            CompleteMultipartUploadRequest completeRequest =
                new()
                {
                    BucketName = s3AwsSettings.BucketName,
                    Key = request.Key,
                    UploadId = initResponse.UploadId,
                };
            completeRequest.AddPartETags(uploadResponses);

            await amazonS3.CompleteMultipartUploadAsync(completeRequest);

            //return new() { S3UploadedPath = GeneratePreSignedURL(request.Key!), Key = request.Key };
            storageResponse.FilePath = GeneratePreSignedURL(request.Key!);
            storageResponse.Key = request.Key;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An AmazonS3Exception was thrown: {ex.Message}");

            AbortMultipartUploadRequest abortMPURequest =
                new()
                {
                    BucketName = s3AwsSettings.BucketName,
                    Key = request.Key,
                    UploadId = initResponse.UploadId,
                };
            await amazonS3.AbortMultipartUploadAsync(abortMPURequest);

            storageResponse.Error = ex.Message;
        }

        return storageResponse;
    }

    public async Task<StorageResponse> DeleteAsync(string key)
    {
        StorageResponse storage = new();
        try
        {
            var request = new DeleteObjectRequest
            {
                BucketName = s3AwsSettings.BucketName,
                Key = key,
            };

            await amazonS3.DeleteObjectAsync(request);
            storage.IsSuccess = true;
        }
        catch (AmazonS3Exception ex)
        {
            storage.Error = ex.Message;
        }

        return storage;
    }

    public string UniqueFileName(string fileName)
    {
        string name = Path.GetFileNameWithoutExtension(fileName).SpecialCharacterRemoving();
        string extension = Path.GetExtension(fileName);
        return $"{name}.{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{extension}";
    }

    public string? GetPublicUrl() => s3AwsSettings.PublicUrl;

    public string? GetFullpath(string? key) =>
        string.IsNullOrWhiteSpace(key) ? null : GeneratePreSignedURL(key);

    private string GeneratePreSignedURL(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = s3AwsSettings.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(
                double.Parse(s3AwsSettings.PreSignedUrlExpirationInMinutes!)
            ),
            Protocol = s3AwsSettings.Protocol,
        };

        string url = amazonS3.GetPreSignedURL(request);

        return url.Replace(
            s3AwsSettings.ServiceUrl!,
            s3AwsSettings.PublicUrl,
            StringComparison.OrdinalIgnoreCase
        );
    }
}
