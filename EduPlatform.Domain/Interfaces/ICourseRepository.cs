using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Interfaces;

/// <summary>
/// Specialized repository contract for Course queries.
///
/// Why specialized instead of using IRepository&lt;Course&gt; directly?
/// The Course module needs multi-level Include chains that would be
/// verbose and duplicated if written in every service or controller.
/// These methods centralize the query logic so:
/// - Services call one method and get a fully loaded Course object.
/// - The Include structure is defined once and changed in one place.
/// - Tests can mock this interface without setting up the full EF Include chain.
///
/// All methods use AsNoTracking internally because they are used for
/// read-only display purposes. Use IUnitOfWork.Courses.GetByIdAsync for
/// tracked entities that need to be updated.
/// </summary>
public interface ICourseRepository : IRepository<Course>
{
    /// <summary>
    /// Returns a fully loaded course for the detail page.
    /// Includes: Subject → Grade, Category, Reviews (with reviewer names),
    ///           Sections → Lessons (ordered by Section.Order, Lesson.Order).
    /// Returns null if the course is not found or is soft-deleted.
    /// </summary>
    Task<Course?> GetWithDetailsAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Returns all courses owned by a specific teacher.
    /// Used in teacher dashboards and course management screens.
    /// Does NOT filter by Status or ApprovalStatus — teachers see their own
    /// draft and rejected courses too.
    /// </summary>
    Task<IReadOnlyList<Course>> GetByTeacherAsync(
        string teacherId,
        CancellationToken ct = default);

    /// <summary>
    /// Returns all courses under a specific subject.
    /// Used to populate the subject detail page and subject-filtered course lists.
    /// Only returns Published + Approved courses visible to the public.
    /// </summary>
    Task<IReadOnlyList<Course>> GetBySubjectAsync(
        int subjectId,
        CancellationToken ct = default);

    /// <summary>
    /// Returns all courses that are both Published AND Approved.
    /// This is the main query used for the public course catalog.
    /// Includes Subject → Grade, Category, and Reviews for display.
    /// Soft-deleted courses are excluded automatically via the global query filter.
    /// </summary>
    Task<IReadOnlyList<Course>> GetPublishedAsync(CancellationToken ct = default);

    /// <summary>
    /// Returns all courses awaiting admin/ContentManager approval.
    /// Used in the PendingTasks page and admin dashboard.
    /// Includes the teacher's basic info for display in the approval list.
    /// </summary>
    Task<IReadOnlyList<Course>> GetPendingApprovalAsync(CancellationToken ct = default);
}
