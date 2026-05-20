using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using EduPlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EduPlatform.Infrastructure.Services.Storage;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient           _client;
    private readonly string                      _accountName;
    private readonly ILogger<BlobStorageService> _log;

    public BlobStorageService(IConfiguration config, ILogger<BlobStorageService> log)
    {
        _log         = log;
        var connStr  = config["AzureStorage:ConnectionString"]
                       ?? throw new InvalidOperationException("AzureStorage:ConnectionString is missing.");
        _accountName = config["AzureStorage:AccountName"]
                       ?? throw new InvalidOperationException("AzureStorage:AccountName is missing.");
        _client      = new BlobServiceClient(connStr);
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, string container, CancellationToken ct = default)
    {
        var containerClient = _client.GetBlobContainerClient(container);
        await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);

        var ext      = Path.GetExtension(fileName).ToLowerInvariant();
        var blobName = $"{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}{ext}";
        var blobClient = containerClient.GetBlobClient(blobName);

        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: ct);
        _log.LogInformation("Blob uploaded: {Url}", blobClient.Uri);
        return blobClient.Uri.ToString();
    }

    public async Task DeleteAsync(string? blobUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(blobUrl) || !IsOwnedUrl(blobUrl)) return;
        try
        {
            var uri      = new Uri(blobUrl);
            var segments = uri.AbsolutePath.TrimStart('/').Split('/', 2);
            if (segments.Length < 2) return;
            var blobClient = _client.GetBlobContainerClient(segments[0]).GetBlobClient(segments[1]);
            await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
            _log.LogInformation("Blob deleted: {Url}", blobUrl);
        }
        catch (Exception ex) { _log.LogWarning(ex, "Failed to delete blob: {Url}", blobUrl); }
    }

    public bool IsOwnedUrl(string? url) =>
        !string.IsNullOrWhiteSpace(url) &&
        url.Contains($"{_accountName}.blob.core.windows.net", StringComparison.OrdinalIgnoreCase);
}
