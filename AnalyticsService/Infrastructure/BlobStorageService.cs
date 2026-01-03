using Azure.Storage.Blobs;
using System.Text;

namespace AnalyticsService.Infrastructure;

public class BlobStorageService
{
    private readonly BlobContainerClient _containerClient;

    public BlobStorageService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlob:ConnectionString"];
        var containerName = configuration["AzureBlob:ContainerName"];

        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists();
    }

    public async Task SaveAsync(string fileName, string content)
    {
        var blobClient = _containerClient.GetBlobClient(fileName);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        await blobClient.UploadAsync(stream, overwrite: true);
    }
}
