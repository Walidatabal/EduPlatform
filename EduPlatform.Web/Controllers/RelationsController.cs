using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using EduPlatform.Web.ViewModels.Relations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// MVC controller for the Relations page.
///
/// Purpose:
/// The Relations page surfaces two important business relationships:
/// 1. Parent → Student: which students a parent is linked to.
/// 2. Teacher → Courses: which courses a teacher owns.
///
/// These relationships drive:
/// - Parent monitoring dashboards (see student progress, enrollments, certificates).
/// - Teacher revenue reports (courses they own and their enrollment counts).
/// - Admin oversight (full view of all relationships across the platform).
///
/// Access control rules:
/// - Admin / ContentManager: sees ALL parent links and ALL teacher courses.
/// - Parent: sees only the students linked to their own account.
/// - Teacher: sees only their own courses.
/// - Student / Anyone else: sees nothing (empty tables displayed).
///
/// Enterprise note:
/// This controller queries AppDbContext directly for the Relations page because
/// it needs to join Identity users (from UserManager) with Domain entities
/// (ParentStudentLink, Course) in a way that does not map cleanly to any
/// existing service method. The direct DbContext usage is acceptable here
/// for read-only display queries — this is NOT a business operation that
/// modifies state.
///
/// Future improvement:
/// Extract into an IRelationsService with a GetRelationsAsync(userId, role)
/// method so the controller stays thin and the query logic is testable.
/// </summary>
[Authorize]
public class RelationsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public RelationsController(AppDbContext db, UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    /// <summary>
    /// GET /Relations
    /// Loads and displays the role-scoped relations view.
    /// </summary>
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        // Identify the current user and their primary role.
        var user  = await _users.GetUserAsync(User);
        var roles = user is not null ? await _users.GetRolesAsync(user) : [];
        var role   = roles.FirstOrDefault() ?? "User";
        var userId = user?.Id ?? string.Empty;
        var isAdmin = role == AppRoles.Admin || role == AppRoles.ContentManager;

        // Pre-load all Identity users into a dictionary for efficient lookup.
        // This avoids N+1 UserManager calls when resolving parent/student/teacher names.
        var allUsers = await _users.Users
            .AsNoTracking()
            .ToDictionaryAsync(x => x.Id, x => x, ct);

        // ── Parent → Student links ─────────────────────────────────────────
        // Admin: all links. Parent: own links. Others: empty result.
        var parentLinksQuery = _db.ParentStudentLinks.AsNoTracking();

        if (!isAdmin && role == AppRoles.Parent)
            parentLinksQuery = parentLinksQuery.Where(x => x.ParentId == userId);
        else if (!isAdmin)
            // Student or Teacher: return empty query — they see no parent links.
            parentLinksQuery = parentLinksQuery.Where(_ => false);

        var parentLinks = await parentLinksQuery.ToListAsync(ct);

        // Map each link to a ViewModel that includes resolved user names and
        // aggregated learning counts (enrollments + certificates).
        var parentStudents = new List<ParentStudentRelationVM>();
        foreach (var link in parentLinks)
        {
            allUsers.TryGetValue(link.ParentId,  out var parent);
            allUsers.TryGetValue(link.StudentId, out var student);

            parentStudents.Add(new ParentStudentRelationVM
            {
                ParentName        = parent?.FullName  ?? "Parent",
                ParentEmail       = parent?.Email     ?? link.ParentId,
                StudentName       = student?.FullName ?? "Student",
                StudentEmail      = student?.Email    ?? link.StudentId,
                RelationshipType  = link.RelationshipType,
                // Aggregate counts: how many courses and certificates the student has.
                EnrollmentsCount  = await _db.Enrollments.CountAsync(e => e.StudentId == link.StudentId, ct),
                CertificatesCount = await _db.Certificates.CountAsync(c => c.StudentId == link.StudentId, ct)
            });
        }

        // ── Teacher → Courses ──────────────────────────────────────────────
        // Admin: all courses. Teacher: own courses. Others: empty result.
        var teacherCoursesQuery = _db.Courses
            .AsNoTracking()
            .Include(c => c.Enrollments)   // needed for EnrollmentsCount
            .AsQueryable();

        if (!isAdmin && role == AppRoles.Teacher)
            teacherCoursesQuery = teacherCoursesQuery.Where(c => c.TeacherId == userId);
        else if (!isAdmin)
            teacherCoursesQuery = teacherCoursesQuery.Where(_ => false);

        var teacherCoursesRaw = await teacherCoursesQuery
            .OrderBy(c => c.TeacherId)     // group by teacher
            .ThenBy(c => c.Title)          // alphabetical within teacher
            .ToListAsync(ct);

        // Map each course to a ViewModel that includes resolved teacher info.
        var teacherCourses = teacherCoursesRaw.Select(course =>
        {
            allUsers.TryGetValue(course.TeacherId, out var teacher);
            return new TeacherCourseRelationVM
            {
                TeacherName      = teacher?.FullName ?? "Teacher",
                TeacherEmail     = teacher?.Email    ?? course.TeacherId,
                CourseTitle      = course.Title,
                Status           = course.Status.ToString(),
                ApprovalStatus   = course.ApprovalStatus.ToString(),
                EnrollmentsCount = course.Enrollments.Count
            };
        }).ToList();

        var vm = new RelationsIndexVM
        {
            CurrentRole   = role,
            ParentStudents = parentStudents,
            TeacherCourses = teacherCourses
        };

        return View(vm);
    }
}
