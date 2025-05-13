using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;

    public BlobService(IConfiguration configuration)
    {
        var connectionString = configuration["AzureBlobStorage:ConnectionString"];
        var containerName = configuration["AzureBlobStorage:ContainerName"];

        if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(containerName))
        {
            throw new InvalidOperationException("Azure Blob Storage settings are missing.");
        }

        _containerClient = new BlobContainerClient(connectionString, containerName);
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("Invalid file provided");

        var blobName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var blobClient = _containerClient.GetBlobClient(blobName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, true);
        }

        // Set the correct content type (so it opens in the browser instead of downloading)
        await blobClient.SetHttpHeadersAsync(new Azure.Storage.Blobs.Models.BlobHttpHeaders
        {
            ContentType = file.ContentType
        });

        return blobClient.Uri.ToString(); // This ensures the URL is stored correctly
    }
    public async Task DeleteFileAsync(string blobUrl)
    {
        try
        {
            var blobUri = new Uri(blobUrl);
            string blobName = Path.GetFileName(blobUri.LocalPath);
            var blobClient = _containerClient.GetBlobClient(blobName);

            await blobClient.DeleteIfExistsAsync();
            Console.WriteLine($"Deleted blob: {blobUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting blob: {ex.Message}");
        }
    }
}