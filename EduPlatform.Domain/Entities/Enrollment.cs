using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

/// <summary>
/// Represents a student's enrollment in a course.
///
/// Business rules:
/// - A student can enroll in a course exactly once (enforced by EnrollmentService
///   via a uniqueness check before insert, not by a DB unique constraint).
/// - <see cref="AmountPaid"/> captures the price at enrollment time (price snapshot).
///   If a teacher later changes the course price, this record is not affected.
///   AmountPaid = 0 means the student enrolled for free.
/// - <see cref="PaidAt"/> is only set when payment was successfully processed
///   through the payment gateway. Null for free enrollments.
/// - <see cref="Status"/> tracks the student's learning lifecycle:
///   Active → student can access lessons.
///   Completed → all lessons marked complete and certificate issued.
///   Dropped → student voluntarily unenrolled (no refund handled here).
///
/// Soft-delete (inherited from BaseEntity.IsDeleted) is used if an admin
/// needs to revoke access without destroying the historical record.
/// </summary>
public class Enrollment : BaseEntity
{
    /// <summary>
    /// Identity GUID of the enrolled student.
    /// Stored as string to keep Domain independent of ASP.NET Core Identity.
    /// </summary>
    public string StudentId { get; set; } = string.Empty;

    /// <summary>Foreign key to the course this enrollment belongs to.</summary>
    public int CourseId { get; set; }

    /// <summary>Navigation property — loaded only when explicitly included.</summary>
    public Course? Course { get; set; }

    /// <summary>
    /// Current lifecycle status of the enrollment.
    /// Default is Active — the student has access to course content.
    /// </summary>
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    /// <summary>
    /// UTC timestamp when the payment was confirmed for a paid course.
    /// Null for free enrollments or enrollments created before payment processing
    /// was implemented.
    /// </summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>
    /// The price the student paid at enrollment time, in Kuwaiti Dinar (KD).
    /// 0 for free courses.
    /// This is the price snapshot — independent of Course.Price changes after enrollment.
    /// </summary>
    public decimal AmountPaid { get; set; }

    /// <summary>
    /// UTC timestamp when the student enrolled.
    /// Set by the service on creation; not auto-set by SaveChangesAsync
    /// (unlike CreatedAt which tracks database-level creation).
    /// Used for "enrolled since" display and cohort analytics.
    /// </summary>
    public DateTime EnrolledAt { get; set; }
}
