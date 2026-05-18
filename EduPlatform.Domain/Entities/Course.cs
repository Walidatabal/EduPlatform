using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

/// <summary>
/// Represents a learning course in the EduPlatform marketplace.
///
/// Business rules encoded here:
/// - A course must belong to a Subject (which belongs to a Grade).
///   This enforces the Grade → Subject → Course hierarchy.
/// - A course has TWO independent publication gates:
///   <see cref="Status"/> (teacher-controlled) and
///   <see cref="ApprovalStatus"/> (admin-controlled).
///   BOTH must be "Published + Approved" before the course is visible to students.
/// - <see cref="TeacherId"/> stores the Identity GUID of the owning teacher.
///   Domain never imports ApplicationUser — it only stores the ID string.
/// - <see cref="Price"/> = 0 means the course is free.
///   Free courses skip the payment step and enroll students directly.
/// - <see cref="ThumbnailUrl"/>, <see cref="Requirements"/>, <see cref="LearningOutcomes"/>
///   are optional and support the full Udemy-style course detail page.
///
/// Navigation properties are initialized to empty collections to prevent
/// NullReferenceException when EF Core has not loaded them (non-tracked contexts).
/// </summary>
public class Course : BaseEntity
{
    /// <summary>Course display title. Required. Max 200 chars enforced by validator.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Long-form course description shown on the course detail page.
    /// Optional — teachers can add this later via Edit.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Absolute URL to the course cover image.
    /// Stored as a string; no file upload — teacher provides a CDN URL.
    /// Validated by CreateCourseCommandValidator to be a valid absolute URI.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Difficulty level. One of: Beginner, Intermediate, Advanced.
    /// Enforced by FluentValidation; not stored as enum to keep display strings flexible.
    /// </summary>
    public string Level { get; set; } = "Beginner";

    /// <summary>
    /// Primary language of the course content. One of: English, Arabic, French.
    /// Affects filtering and UI language hints — does not affect the portal UI language.
    /// </summary>
    public string Language { get; set; } = "English";

    /// <summary>Prerequisites the student should meet before enrolling. Optional.</summary>
    public string? Requirements { get; set; }

    /// <summary>What the student will learn. Used in the course detail bullet list. Optional.</summary>
    public string? LearningOutcomes { get; set; }

    /// <summary>
    /// Optional foreign key to Category.
    /// Categories provide a secondary classification independent of Grade/Subject.
    /// Nullable because some courses may belong to a subject without a category.
    /// </summary>
    public int? CategoryId { get; set; }

    /// <summary>Navigation property — loaded only when explicitly included.</summary>
    public Category? Category { get; set; }

    /// <summary>
    /// Course price in Kuwaiti Dinar (KD).
    /// 0 = free course. Max 10,000 KD enforced by validator.
    /// The cart stores a PriceSnapshot so changes here never affect existing cart/order records.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Teacher-controlled publication gate.
    /// Draft = course is in preparation (not visible to students).
    /// Published = teacher declared the course ready for review.
    /// Students only see the course if BOTH Status=Published AND ApprovalStatus=Approved.
    /// </summary>
    public CourseStatus Status { get; set; } = CourseStatus.Draft;

    /// <summary>
    /// Admin/ContentManager-controlled approval gate.
    /// Pending = waiting for review.
    /// Approved = admin verified quality and approved publication.
    /// Rejected = admin rejected; teacher must revise and resubmit.
    /// </summary>
    public ApprovalStatus ApprovalStatus { get; set; } = ApprovalStatus.Pending;

    /// <summary>
    /// Identity GUID of the teacher who created this course.
    /// Stored as string to keep Domain independent of ASP.NET Core Identity.
    /// Used for filtering in teacher dashboards and ownership checks in commands.
    /// </summary>
    public string TeacherId { get; set; } = string.Empty;

    /// <summary>Foreign key to Subject. Required — every course belongs to one subject.</summary>
    public int SubjectId { get; set; }

    /// <summary>Navigation property to the owning subject (and transitively its grade).</summary>
    public Subject? Subject { get; set; }

    /// <summary>
    /// Ordered list of course sections (modules).
    /// A section contains an ordered list of lessons.
    /// Initialized empty to prevent NullReferenceException on non-tracked contexts.
    /// </summary>
    public ICollection<Section> Sections { get; set; } = new List<Section>();

    /// <summary>
    /// All enrollments on this course.
    /// Count used for the enrollment counter on the course detail page.
    /// </summary>
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    /// <summary>
    /// Student reviews for this course.
    /// AverageRating is computed from this collection in AutoMapper profile.
    /// </summary>
    public ICollection<CourseReview> Reviews { get; set; } = new List<CourseReview>();

    /// <summary>Scheduled live sessions associated with this course.</summary>
    public ICollection<LiveSession> LiveSessions { get; set; } = new List<LiveSession>();
}
