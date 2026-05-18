namespace EduPlatform.Web.ViewModels.Relations;

/// <summary>
/// Root ViewModel for the Relations page (/Relations).
///
/// This ViewModel holds two independent lists:
/// - <see cref="ParentStudents"/>: parent/student relationship records.
/// - <see cref="TeacherCourses"/>: teacher/course relationship records.
///
/// Both lists are role-scoped:
/// - Admin/ContentManager: sees all records across the entire platform.
/// - Parent: sees only their own linked students.
/// - Teacher: sees only their own courses.
/// - Other roles: sees empty lists (no data shown).
///
/// <see cref="CurrentRole"/> is passed to the view to adjust heading text
/// and empty-state messages based on who is viewing.
/// </summary>
public class RelationsIndexVM
{
    /// <summary>
    /// The primary role of the currently authenticated user.
    /// Examples: "Admin", "Teacher", "Parent".
    /// Used in the view to customize section headings and empty-state messages.
    /// </summary>
    public string CurrentRole { get; set; } = string.Empty;

    /// <summary>
    /// List of parent/student relationship rows to display.
    /// Empty for roles that are not Admin, ContentManager, or Parent.
    /// </summary>
    public IReadOnlyList<ParentStudentRelationVM> ParentStudents { get; set; } = [];

    /// <summary>
    /// List of teacher/course relationship rows to display.
    /// Empty for roles that are not Admin, ContentManager, or Teacher.
    /// </summary>
    public IReadOnlyList<TeacherCourseRelationVM> TeacherCourses { get; set; } = [];
}

/// <summary>
/// Represents one row in the Parent → Student table.
///
/// This ViewModel combines data from three sources:
/// 1. ParentStudentLink entity (RelationshipType).
/// 2. UserManager (ParentName, ParentEmail, StudentName, StudentEmail).
/// 3. Aggregated counts (EnrollmentsCount, CertificatesCount).
///
/// The names and emails are resolved in RelationsController by looking up
/// the ParentId and StudentId GUIDs in the allUsers dictionary.
/// </summary>
public class ParentStudentRelationVM
{
    /// <summary>Full name of the parent user. Falls back to "Parent" if not found.</summary>
    public string ParentName { get; set; } = string.Empty;

    /// <summary>Email of the parent user. Falls back to the raw ParentId GUID if not found.</summary>
    public string ParentEmail { get; set; } = string.Empty;

    /// <summary>Full name of the student. Falls back to "Student" if not found.</summary>
    public string StudentName { get; set; } = string.Empty;

    /// <summary>Email of the student. Falls back to the raw StudentId GUID if not found.</summary>
    public string StudentEmail { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable relationship type.
    /// Examples: Father, Mother, Guardian.
    /// Displayed as a badge in the Relations table.
    /// </summary>
    public string RelationshipType { get; set; } = string.Empty;

    /// <summary>
    /// Total number of active course enrollments for this student.
    /// Queried with COUNT(*) from the Enrollments table in RelationsController.
    /// Used by parents to quickly see how many courses their child is taking.
    /// </summary>
    public int EnrollmentsCount { get; set; }

    /// <summary>
    /// Total number of certificates issued to this student.
    /// Queried with COUNT(*) from the Certificates table in RelationsController.
    /// Used by parents to see their child's achievements.
    /// </summary>
    public int CertificatesCount { get; set; }
}

/// <summary>
/// Represents one row in the Teacher → Courses table.
///
/// This ViewModel combines data from two sources:
/// 1. Course entity (CourseTitle, Status, ApprovalStatus, EnrollmentsCount).
/// 2. UserManager (TeacherName, TeacherEmail — resolved from TeacherId GUID).
///
/// Status and ApprovalStatus are stored as strings (not enums) because they
/// are only used for display (badges) in the view, not for business logic.
/// </summary>
public class TeacherCourseRelationVM
{
    /// <summary>Full name of the teacher. Falls back to "Teacher" if not found.</summary>
    public string TeacherName { get; set; } = string.Empty;

    /// <summary>Email of the teacher. Falls back to the raw TeacherId GUID if not found.</summary>
    public string TeacherEmail { get; set; } = string.Empty;

    /// <summary>Course display title.</summary>
    public string CourseTitle { get; set; } = string.Empty;

    /// <summary>
    /// Course publication status as a string.
    /// Examples: "Draft", "Published".
    /// Displayed as a badge in the Relations table.
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Course approval status as a string.
    /// Examples: "Pending", "Approved", "Rejected".
    /// Combined with Status in the badge display.
    /// </summary>
    public string ApprovalStatus { get; set; } = string.Empty;

    /// <summary>
    /// Total number of students enrolled in this course.
    /// Loaded via .Include(c => c.Enrollments).Count in RelationsController.
    /// Used to show teacher influence and course popularity.
    /// </summary>
    public int EnrollmentsCount { get; set; }
}
