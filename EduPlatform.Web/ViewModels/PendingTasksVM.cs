namespace EduPlatform.Web.ViewModels;

/// <summary>
/// ViewModel for the Pending Tasks admin page (/PendingTasks).
///
/// Purpose:
/// The PendingTasks page gives Admin and ContentManager a single place to
/// action all items that require human approval or intervention:
/// 1. Teachers who registered with the PendingTeacher role and await promotion to Teacher.
/// 2. Courses that have been submitted for review (ApprovalStatus = Pending).
/// 3. User accounts that have been locked out (5 failed login attempts → 15 min lockout).
///
/// TotalPending:
/// The computed property TotalPending sums all three lists.
/// It is displayed in the sidebar as a badge ("Pending Tasks 5") so admins
/// know at a glance whether there are items requiring their attention.
///
/// Access: [Authorize(Roles = "Admin,ContentManager")] on PendingTasksController.
/// Students and Teachers never see this page.
/// </summary>
public class PendingTasksVM
{
    /// <summary>
    /// Teachers who registered with the PendingTeacher role.
    /// Admin can approve (promote to Teacher role) or reject (keep or delete account).
    /// </summary>
    public List<PendingTeacherVM> PendingTeachers { get; set; } = [];

    /// <summary>
    /// Courses submitted by teachers that have ApprovalStatus = Pending.
    /// Admin/ContentManager must approve or reject each course before it
    /// becomes visible to students.
    /// </summary>
    public List<PendingCourseVM> PendingCourses { get; set; } = [];

    /// <summary>
    /// Users currently locked out due to too many failed login attempts.
    /// Admin can unlock an account before the automatic lockout expiry (15 minutes).
    /// </summary>
    public List<LockedUserVM> LockedUsers { get; set; } = [];

    /// <summary>
    /// Total number of items requiring admin action across all categories.
    /// Used for the sidebar badge count and the page header total.
    /// </summary>
    public int TotalPending =>
        PendingTeachers.Count + PendingCourses.Count + LockedUsers.Count;
}

/// <summary>Minimal info for a teacher awaiting approval.</summary>
public class PendingTeacherVM
{
    /// <summary>Identity user GUID. Used as the route parameter in Approve/Reject actions.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Teacher's display name from ApplicationUser.FullName.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Teacher's email address. Used to identify the account in the list.</summary>
    public string Email { get; set; } = string.Empty;
}

/// <summary>Minimal info for a course awaiting content review.</summary>
public class PendingCourseVM
{
    /// <summary>Course primary key. Used as the route parameter in Approve/Reject actions.</summary>
    public int CourseId { get; set; }

    /// <summary>Course display title shown in the pending tasks table.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Identity GUID of the teacher who submitted the course.</summary>
    public string TeacherId { get; set; } = string.Empty;

    /// <summary>Resolved teacher name for display (loaded from UserManager in the controller).</summary>
    public string TeacherName { get; set; } = string.Empty;

    /// <summary>UTC timestamp of course creation — shown to help prioritize older submissions.</summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>Minimal info for a locked-out user account.</summary>
public class LockedUserVM
{
    /// <summary>Identity user GUID. Used as the route parameter in the Unlock action.</summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>User's display name from ApplicationUser.FullName.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>User's email address. Used to identify the account.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// When the lockout will automatically expire.
    /// Displayed to help admin decide whether to unlock early or let it expire.
    /// Null means account was manually locked (LockoutEnd = DateTimeOffset.MaxValue).
    /// </summary>
    public DateTimeOffset? LockoutEnd { get; set; }
}
