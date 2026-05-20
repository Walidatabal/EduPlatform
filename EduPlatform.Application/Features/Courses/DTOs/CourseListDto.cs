using EduPlatform.Domain.Enums;

namespace EduPlatform.Application.Features.Courses.DTOs;

public class CourseListDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public decimal AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public CourseStatus Status { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string GradeName { get; set; } = string.Empty;
    public int EnrollmentCount { get; set; }
}
