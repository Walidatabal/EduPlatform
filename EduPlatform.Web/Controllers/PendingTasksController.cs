using EduPlatform.Domain.Constants;
using EduPlatform.Domain.Enums;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using EduPlatform.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// Admin pending tasks controller.
/// 
/// Handles operational tasks that require admin/content manager attention:
/// - Pending teacher approval
/// - Pending course approval
/// - Locked accounts
/// </summary>
[Authorize(Roles = AppRoles.Admin + "," + AppRoles.ContentManager)]
public class PendingTasksController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public PendingTasksController(
        AppDbContext db,
        UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var pendingTeachers = await _users.GetUsersInRoleAsync(AppRoles.PendingTeacher);

        var lockedUsers = await _users.Users
            .Where(u => u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow)
            .OrderBy(u => u.Email)
            .ToListAsync(ct);

        var pendingCourses = await _db.Courses
            .AsNoTracking()
            .Where(c => c.ApprovalStatus == ApprovalStatus.Pending)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new PendingCourseVM
            {
                CourseId = c.Id,
                Title = c.Title,
                TeacherId = c.TeacherId,
                TeacherName = c.TeacherId,
                CreatedAt = c.CreatedAt
            })
            .ToListAsync(ct);

        var vm = new PendingTasksVM
        {
            PendingTeachers = pendingTeachers.Select(t => new PendingTeacherVM
            {
                UserId = t.Id,
                FullName = t.FullName,
                Email = t.Email ?? string.Empty
            }).ToList(),

            PendingCourses = pendingCourses,

            LockedUsers = lockedUsers.Select(u => new LockedUserVM
            {
                UserId = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                LockoutEnd = u.LockoutEnd
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveTeacher(string id)
    {
        var user = await _users.FindByIdAsync(id);

        if (user is null)
        {
            TempData["Error"] = "Teacher user not found.";
            return RedirectToAction(nameof(Index));
        }

        if (await _users.IsInRoleAsync(user, AppRoles.PendingTeacher))
        {
            await _users.RemoveFromRoleAsync(user, AppRoles.PendingTeacher);
        }

        if (!await _users.IsInRoleAsync(user, AppRoles.Teacher))
        {
            await _users.AddToRoleAsync(user, AppRoles.Teacher);
        }

        TempData["Success"] = $"Teacher approved: {user.Email}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectTeacher(string id)
    {
        var user = await _users.FindByIdAsync(id);

        if (user is null)
        {
            TempData["Error"] = "Teacher user not found.";
            return RedirectToAction(nameof(Index));
        }

        if (await _users.IsInRoleAsync(user, AppRoles.PendingTeacher))
        {
            await _users.RemoveFromRoleAsync(user, AppRoles.PendingTeacher);
        }

        if (!await _users.IsInRoleAsync(user, AppRoles.Student))
        {
            await _users.AddToRoleAsync(user, AppRoles.Student);
        }

        TempData["Success"] = $"Teacher request rejected: {user.Email}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveCourse(int id, CancellationToken ct)
    {
        var course = await _db.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);

        if (course is null)
        {
            TempData["Error"] = "Course not found.";
            return RedirectToAction(nameof(Index));
        }

        course.ApprovalStatus = ApprovalStatus.Approved;
        course.Status = CourseStatus.Published;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Course approved: {course.Title}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectCourse(int id, CancellationToken ct)
    {
        var course = await _db.Courses.FirstOrDefaultAsync(c => c.Id == id, ct);

        if (course is null)
        {
            TempData["Error"] = "Course not found.";
            return RedirectToAction(nameof(Index));
        }

        course.ApprovalStatus = ApprovalStatus.Rejected;
        course.Status = CourseStatus.Draft;

        await _db.SaveChangesAsync(ct);

        TempData["Success"] = $"Course rejected: {course.Title}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UnlockUser(string id)
    {
        var user = await _users.FindByIdAsync(id);

        if (user is null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction(nameof(Index));
        }

        await _users.SetLockoutEndDateAsync(user, null);
        await _users.ResetAccessFailedCountAsync(user);

        TempData["Success"] = $"Account unlocked: {user.Email}";
        return RedirectToAction(nameof(Index));
    }
}
