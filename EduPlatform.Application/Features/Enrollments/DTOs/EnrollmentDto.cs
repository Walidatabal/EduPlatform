using EduPlatform.Domain.Enums;

namespace EduPlatform.Application.Features.Enrollments.DTOs;

public class EnrollmentDto
{
    public int Id { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public EnrollmentStatus Status { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime CreatedAt { get; set; }
}
