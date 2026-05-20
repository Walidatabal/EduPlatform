using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using EduPlatform.Web.ViewModels.LiveSessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Web.Controllers;

[Authorize]
public class LiveSessionsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public LiveSessionsController(AppDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db          = db;
        _userManager = userManager;
    }

    // ── Session list ──────────────────────────────────────────────────────────

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId            = _userManager.GetUserId(User);
        var canManage         = User.IsInRole(AppRoles.Teacher) || User.IsInRole(AppRoles.Admin);

        var sessions = await _db.LiveSessions
            .AsNoTracking()
            .Include(l => l.Course)
            .OrderBy(l => l.StartTime)
            .Select(l => new LiveSessionItemVM
            {
                Id           = l.Id,
                CourseId     = l.CourseId,
                CourseTitle  = l.Course != null ? l.Course.Title : $"Course #{l.CourseId}",
                Title        = l.Title,
                Description  = l.Description,
                StartTime    = l.StartTime,
                EndTime      = l.EndTime,
                Status       = l.Status.ToString(),
                MeetingUrl   = l.MeetingUrl,
                IsInstructor = l.InstructorId == userId
            })
            .ToListAsync(ct);

        return View(new LiveSessionIndexVM { Sessions = sessions, CanManageSessions = canManage });
    }

    // ── Attendance roster (Teacher / Admin) ───────────────────────────────────

    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> Attendance(int id, CancellationToken ct)
    {
        var session = await _db.LiveSessions
            .AsNoTracking()
            .Include(l => l.Attendances)
            .Include(l => l.Course)
            .FirstOrDefaultAsync(l => l.Id == id, ct);

        if (session == null) return NotFound();

        var userId  = _userManager.GetUserId(User);
        var isAdmin = User.IsInRole(AppRoles.Admin);
        if (!isAdmin && session.InstructorId != userId) return Forbid();

        var studentIds = session.Attendances.Select(a => a.StudentId).ToList();
        var users = await _db.Users
            .Where(u => studentIds.Contains(u.Id))
            .Select(u => new { u.Id, Name = u.UserName ?? u.Email ?? u.Id })
            .ToDictionaryAsync(u => u.Id, u => u.Name, ct);

        var vm = new LiveSessionAttendanceVM
        {
            SessionId    = session.Id,
            SessionTitle = session.Title,
            CourseTitle  = session.Course?.Title ?? string.Empty,
            StartTime    = session.StartTime,
            Status       = session.Status.ToString(),
            Attendees    = session.Attendances
                .OrderBy(a => a.JoinedAt ?? DateTime.MaxValue)
                .Select(a => new AttendeeRowVM
                {
                    AttendanceId    = a.Id,
                    StudentId       = a.StudentId,
                    StudentName     = users.GetValueOrDefault(a.StudentId, a.StudentId),
                    Status          = a.Status.ToString(),
                    JoinedAt        = a.JoinedAt,
                    LeftAt          = a.LeftAt,
                    DurationMinutes = a.DurationMinutes,
                    Note            = a.Note
                })
                .ToList()
        };

        return View(vm);
    }

    // ── Student attendance history ────────────────────────────────────────────

    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> MyAttendance(CancellationToken ct)
    {
        var userId = _userManager.GetUserId(User);

        var records = await _db.SessionAttendances
            .AsNoTracking()
            .Include(a => a.LiveSession).ThenInclude(l => l!.Course)
            .Where(a => a.StudentId == userId)
            .OrderByDescending(a => a.JoinedAt)
            .Select(a => new MyAttendanceRowVM
            {
                SessionId       = a.LiveSessionId,
                SessionTitle    = a.LiveSession != null ? a.LiveSession.Title : $"Session #{a.LiveSessionId}",
                CourseTitle     = a.LiveSession != null && a.LiveSession.Course != null ? a.LiveSession.Course.Title : string.Empty,
                SessionDate     = a.LiveSession != null ? a.LiveSession.StartTime : default,
                Status          = a.Status.ToString(),
                JoinedAt        = a.JoinedAt,
                LeftAt          = a.LeftAt,
                DurationMinutes = a.DurationMinutes
            })
            .ToListAsync(ct);

        return View(new MyAttendanceVM { Records = records });
    }

    // ── GoLive POST ───────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> GoLive(int id, CancellationToken ct)
    {
        var session = await _db.LiveSessions.FindAsync([id], ct);
        if (session == null) return NotFound();
        var userId = _userManager.GetUserId(User);
        if (!User.IsInRole(AppRoles.Admin) && session.InstructorId != userId) return Forbid();
        try { session.GoLive(); await _db.SaveChangesAsync(ct); }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }

    // ── Complete POST ─────────────────────────────────────────────────────────

    [HttpPost, ValidateAntiForgeryToken]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> Complete(int id, CancellationToken ct)
    {
        var session = await _db.LiveSessions.Include(l => l.Attendances)
            .FirstOrDefaultAsync(l => l.Id == id, ct);
        if (session == null) return NotFound();
        var userId = _userManager.GetUserId(User);
        if (!User.IsInRole(AppRoles.Admin) && session.InstructorId != userId) return Forbid();
        try { session.Complete(); await _db.SaveChangesAsync(ct); }
        catch (InvalidOperationException ex) { TempData["Error"] = ex.Message; }
        return RedirectToAction(nameof(Index));
    }
}
