using Microsoft.AspNetCore.Http;

namespace Application.SubcutaneousTests.Extensions;

public static class FileHelper
{
    public static IFormFile GenerateIFormfile(string filePath)
    {
        // Read the file into a byte array.
        byte[] fileBytes = File.ReadAllBytes(filePath);

        // Create a new MemoryStream from the byte array.
        MemoryStream memoryStream = new(fileBytes);

        // Create a new FormFile using the MemoryStream and the file name.
        IFormFile formFile = new FormFile(
            memoryStream,
            0,
            fileBytes.Length,
            Path.GetFileName(filePath),
            Path.GetFileName(filePath)
        );

        return formFile;
    }
}
