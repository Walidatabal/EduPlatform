using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Application.Features.Courses.Commands;

public class CreateCourseCommand
{
    [Required, MaxLength(200)] public string Title { get; set; } = string.Empty;
    [MaxLength(2000)] public string? Description { get; set; }
    [Range(0, 10000)] public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string Level { get; set; } = "Beginner";
    public string Language { get; set; } = "English";
    public string? Requirements { get; set; }
    public string? LearningOutcomes { get; set; }
    public int? CategoryId { get; set; }
    [Required] public int SubjectId { get; set; }
}
