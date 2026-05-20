using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using EduPlatform.Web.ViewModels.Enrollments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// MVC controller for learning enrollments.
/// 
/// Student sees personal enrollments.
/// Admin/ContentManager can use it as a testing overview.
/// </summary>
[Authorize]
public class EnrollmentsController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public EnrollmentsController(AppDbContext db, UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var user = await _users.GetUserAsync(User);
        var roles = user is not null ? await _users.GetRolesAsync(user) : [];
        var userId = GetCurrentUserId();

        var isAdmin = roles.Contains(AppRoles.Admin) || roles.Contains(AppRoles.ContentManager);

        var query = _db.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .AsQueryable();

        if (!isAdmin)
            query = query.Where(e => e.StudentId == userId);

        var vm = new EnrollmentIndexVM
        {
            Enrollments = await query
                .OrderByDescending(e => e.EnrolledAt)
                .Select(e => new EnrollmentItemVM
                {
                    CourseId = e.CourseId,
                    CourseTitle = e.Course != null ? e.Course.Title : $"Course #{e.CourseId}",
                    Status = e.Status.ToString(),
                    EnrolledAt = e.EnrolledAt,
                    AmountPaid = e.AmountPaid
                })
                .ToListAsync(ct)
        };

        return View(vm);
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }
}
