using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Application.Features.Lessons.Commands;

public class CreateLessonCommand
{
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public int DurationSeconds { get; set; }
    public int Order { get; set; }
    public bool IsFreePreview { get; set; }
    public int SectionId { get; set; }
}
