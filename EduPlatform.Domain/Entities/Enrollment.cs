using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

public class Enrollment : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public DateTime? PaidAt { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime EnrolledAt { get; set; }
}
