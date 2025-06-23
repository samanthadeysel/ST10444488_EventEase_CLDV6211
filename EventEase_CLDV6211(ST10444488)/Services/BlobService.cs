using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
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

        _containerClient = new BlobContainerClient(connectionString, containerName);
        _containerClient.CreateIfNotExists(); 
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        var blobClient = _containerClient.GetBlobClient(fileName);

        using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
        {
            ContentType = file.ContentType
        });

        return blobClient.Uri.ToString();
    }

    public async Task<string> ReplaceFileAsync(string existingBlobUrl, IFormFile newFile)
    {
        if (!string.IsNullOrEmpty(existingBlobUrl))
        {
            await DeleteFileAsync(existingBlobUrl);
        }

        return await UploadFileAsync(newFile);
    }

    public async Task DeleteFileAsync(string blobUrl)
    {
        if (string.IsNullOrWhiteSpace(blobUrl))
            return;

        var blobName = Path.GetFileName(new Uri(blobUrl).LocalPath);
        var blobClient = _containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }
}