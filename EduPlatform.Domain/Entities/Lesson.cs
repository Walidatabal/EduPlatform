using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

public class Lesson : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public int DurationSeconds { get; set; }
    public int Order { get; set; }
    public bool IsFreePreview { get; set; }
    public string ContentType { get; set; } = "Video";
    public string? ArticleHtml { get; set; }
    public string? ResourceUrl { get; set; }
    public int SectionId { get; set; }
    public Section? Section { get; set; }
}
