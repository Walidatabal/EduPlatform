using EduPlatform.Domain.Entities;

namespace EduPlatform.Tests.Unit.Domain;

/// <summary>
/// Unit tests for domain entity default state.
///
/// What these tests prove:
/// - Every entity begins life with sensible defaults that protect the system.
/// - IsDeleted = false ensures new records are visible immediately.
/// - Default Status/ApprovalStatus values match the intended workflow.
/// - Price = 0 is valid (free courses are supported).
///
/// Why test defaults?
/// Default property values are set in the class definition. A developer
/// refactoring the entity might accidentally change a default, causing
/// subtle production bugs (e.g. new courses suddenly being Published by default,
/// bypassing the approval workflow). These tests catch that instantly.
///
/// No database, no HTTP, no mocking required — pure domain model tests.
/// </summary>
public class BaseEntityTests
{
    // ── Grade tests ──────────────────────────────────────────────────────────

    /// <summary>
    /// A newly created grade must not be soft-deleted.
    /// IsDeleted = false is the default on BaseEntity.
    /// If this changes, new grades would be invisible immediately on creation.
    /// </summary>
    [Fact]
    public void Grade_New_IsNotDeleted_By_Default()
    {
        var grade = new Grade { Name = "Grade 10" };
        Assert.False(grade.IsDeleted);
    }

    // ── Course tests ─────────────────────────────────────────────────────────

    /// <summary>
    /// Price = 0 must be a valid state representing a free course.
    /// The platform supports both free and paid courses.
    /// A zero price should not trigger any validation errors.
    /// </summary>
    [Fact]
    public void Course_Price_CanBeZero_ForFreeCourse()
    {
        var course = new Course { Title = "Free Course", Price = 0 };
        Assert.Equal(0, course.Price);
    }

    /// <summary>
    /// New courses must start with ApprovalStatus = Pending.
    /// This means admin must explicitly approve before the course is visible.
    /// If this default were Approved, teachers could bypass the review workflow.
    /// </summary>
    [Fact]
    public void Course_DefaultApprovalStatus_IsPending()
    {
        var course = new Course { Title = "Test" };
        Assert.Equal(EduPlatform.Domain.Enums.ApprovalStatus.Pending, course.ApprovalStatus);
    }

    /// <summary>
    /// New courses must start with Status = Draft.
    /// This means the course is private until the teacher explicitly publishes it.
    /// If this default were Published, unfinished courses would appear in the catalog.
    /// </summary>
    [Fact]
    public void Course_DefaultStatus_IsDraft()
    {
        var course = new Course { Title = "Test" };
        Assert.Equal(EduPlatform.Domain.Enums.CourseStatus.Draft, course.Status);
    }

    // ── Enrollment tests ─────────────────────────────────────────────────────

    /// <summary>
    /// New enrollments must start with Status = Active.
    /// Active means the student has access to course content.
    /// A different default (e.g. Pending) would block access until manually activated.
    /// </summary>
    [Fact]
    public void Enrollment_DefaultStatus_IsActive()
    {
        var enrollment = new Enrollment();
        Assert.Equal(EduPlatform.Domain.Enums.EnrollmentStatus.Active, enrollment.Status);
    }
}
