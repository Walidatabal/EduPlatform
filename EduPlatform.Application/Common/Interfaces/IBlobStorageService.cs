namespace EduPlatform.Application.Common.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream stream, string fileName, string contentType, string container, CancellationToken ct = default);
    Task DeleteAsync(string? blobUrl, CancellationToken ct = default);
    bool IsOwnedUrl(string? url);
}
