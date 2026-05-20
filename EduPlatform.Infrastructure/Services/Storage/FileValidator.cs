using Microsoft.AspNetCore.Http;

namespace EduPlatform.Infrastructure.Services.Storage;

public static class FileValidator
{
    private static readonly HashSet<string> ImageTypes = new(StringComparer.OrdinalIgnoreCase)
        { "image/jpeg", "image/jpg", "image/png", "image/webp", "image/gif" };

    private static readonly HashSet<string> DocumentTypes = new(StringComparer.OrdinalIgnoreCase)
        { "application/pdf" };

    private const long MaxImageBytes    = 5  * 1024 * 1024;
    private const long MaxDocumentBytes = 20 * 1024 * 1024;

    public const string CourseThumbnails = "course-thumbnails";
    public const string Avatars          = "avatars";
    public const string Certificates     = "certificates";
    public const string LessonFiles      = "lesson-files";

    public static string? ValidateImage(IFormFile? file)
    {
        if (file is null || file.Length == 0) return "Please select a file.";
        if (!ImageTypes.Contains(file.ContentType)) return "Only JPEG, PNG, WebP, or GIF images are allowed.";
        if (file.Length > MaxImageBytes) return "Image must be smaller than 5 MB.";
        return null;
    }

    public static string? ValidateDocument(IFormFile? file)
    {
        if (file is null || file.Length == 0) return "Please select a file.";
        if (!DocumentTypes.Contains(file.ContentType)) return "Only PDF documents are allowed.";
        if (file.Length > MaxDocumentBytes) return "Document must be smaller than 20 MB.";
        return null;
    }
}
