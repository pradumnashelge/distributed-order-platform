using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace AnalyticsService.Infrastructure;

public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlob:ConnectionString"];
        var containerName = configuration["AzureBlob:ContainerName"];

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("AzureBlob:ConnectionString is missing or empty.");

        if (string.IsNullOrWhiteSpace(containerName))
            throw new InvalidOperationException("AzureBlob:ContainerName is missing or empty.");

        _containerClient = new BlobContainerClient(connectionString, containerName);

        // Safe to call multiple times
        _containerClient.CreateIfNotExists();
    }

    public async Task SaveAsync(string fileName, string content)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await blobClient.UploadAsync(stream, overwrite: true);
    }
}
