using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.API.Services;

public class BlobService
{
    private readonly string _connectionString;
    private readonly string _containerName;

    public BlobService(IConfiguration config)
    {
        _connectionString = config["AzureStorage:ConnectionString"]!;
        _containerName = config["AzureStorage:ContainerName"]!;
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var blobServiceClient = new BlobServiceClient(_connectionString);
        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_containerName);

        // Ensure container exists (just in case)
        await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

        // Give the file a unique name so files don't overwrite each other
        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var blobClient = blobContainerClient.GetBlobClient(fileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = file.ContentType });

        return blobClient.Uri.ToString();
    }
}