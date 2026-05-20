using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

public class CourseReview : BaseEntity
{
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public bool IsApproved { get; set; } = true;
}
