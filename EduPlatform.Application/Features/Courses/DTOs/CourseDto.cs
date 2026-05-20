using EduPlatform.Domain.Enums;

namespace EduPlatform.Application.Features.Courses.DTOs;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string? Requirements { get; set; }
    public string? LearningOutcomes { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public CourseStatus Status { get; set; }
    public ApprovalStatus ApprovalStatus { get; set; }
    public string TeacherId { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string GradeName { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
    public int SectionCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
