using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string Level { get; set; } = "Beginner";
    public string Language { get; set; } = "English";
    public string? Requirements { get; set; }
    public string? LearningOutcomes { get; set; }
    public int? CategoryId { get; set; }
    public Category? Category { get; set; }
    public decimal Price { get; set; }
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;
    public string TeacherId { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public ICollection<Section> Sections { get; set; } = new List<Section>();
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();
    public ICollection<LiveSession> LiveSessions { get; set; } = new List<LiveSession>();
}
