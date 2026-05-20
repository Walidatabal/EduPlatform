using EduPlatform.Domain.Constants;
using EduPlatform.Domain.Enums;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using EduPlatform.Web.ViewModels.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// Role-aware dashboard controller.
/// 
/// Enterprise rule:
/// - Controller prepares dashboard data only.
/// - Complex business operations remain in services.
/// - The same dashboard view adapts based on user role.
/// </summary>
[Authorize]
public class DashboardController : Controller
{
    private readonly AppDbContext _db;
    private readonly UserManager<ApplicationUser> _users;

    public DashboardController(AppDbContext db, UserManager<ApplicationUser> users)
    {
        _db = db;
        _users = users;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var user = await _users.GetUserAsync(User);
        var roles = user is not null
            ? await _users.GetRolesAsync(user)
            : [];

        var role = roles.FirstOrDefault() ?? "User";
        var userId = user?.Id ?? string.Empty;

        var recentCourses = await _db.Courses
            .AsNoTracking()
            .Include(c => c.Category)
            .Where(c => role != AppRoles.Teacher || c.TeacherId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .Take(6)
            .Select(c => new RecentCourseItem
            {
                Id = c.Id,
                Title = c.Title,
                Category = c.Category != null ? c.Category.Name : "—",
                Level = c.Level,
                Price = c.Price,
                Status = c.Status.ToString(),
                ApprovalStatus = c.ApprovalStatus.ToString()
            })
            .ToListAsync(ct);

        var recentEnrollments = await _db.Enrollments
            .AsNoTracking()
            .Include(e => e.Course)
            .Where(e =>
                role == AppRoles.Admin ||
                role == AppRoles.ContentManager ||
                e.StudentId == userId ||
                (role == AppRoles.Teacher && e.Course != null && e.Course.TeacherId == userId))
            .OrderByDescending(e => e.EnrolledAt)
            .Take(6)
            .Select(e => new RecentEnrollmentItem
            {
                CourseTitle = e.Course != null ? e.Course.Title : $"Course #{e.CourseId}",
                StudentName = e.StudentId,
                EnrolledAt = e.EnrolledAt,
                Status = e.Status.ToString()
            })
            .ToListAsync(ct);

        var recentOrders = await _db.Orders
            .AsNoTracking()
            .Where(o => role == AppRoles.Admin || role == AppRoles.ContentManager || o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .Select(o => new RecentOrderItem
            {
                Id = o.Id,
                Total = o.Total,
                Status = o.Status.ToString(),
                CreatedAt = o.CreatedAt
            })
            .ToListAsync(ct);

        var notifications = await _db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .Select(n => new NotificationItem
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(ct);

        var upcomingSessions = await _db.LiveSessions
            .AsNoTracking()
            .Include(l => l.Course)
            .Where(l =>
                l.StartTime >= DateTime.UtcNow &&
                (role != AppRoles.Teacher || l.InstructorId == userId))
            .OrderBy(l => l.StartTime)
            .Take(5)
            .Select(l => new LiveSessionItem
            {
                Id = l.Id,
                Title = l.Title,
                CourseTitle = l.Course != null ? l.Course.Title : $"Course #{l.CourseId}",
                StartTime = l.StartTime,
                Status = l.Status.ToString()
            })
            .ToListAsync(ct);

        var vm = new DashboardVM
        {
            UserFullName = user?.FullName ?? user?.Email ?? "User",
            UserEmail = user?.Email ?? string.Empty,
            UserRole = role,

            UsersCount = await _users.Users.CountAsync(ct),
            StudentsCount = (await _users.GetUsersInRoleAsync(AppRoles.Student)).Count,
            TeachersCount = (await _users.GetUsersInRoleAsync(AppRoles.Teacher)).Count,

            GradesCount = await _db.Grades.CountAsync(ct),
            SubjectsCount = await _db.Subjects.CountAsync(ct),
            CoursesCount = await _db.Courses.CountAsync(ct),
            PublishedCoursesCount = await _db.Courses.CountAsync(c => c.Status == CourseStatus.Published, ct),
            PendingCoursesCount = await _db.Courses.CountAsync(c => c.ApprovalStatus == ApprovalStatus.Pending, ct),
            EnrollmentsCount = await _db.Enrollments.CountAsync(ct),
            OrdersCount = await _db.Orders.CountAsync(ct),
            PaymentsCount = await _db.Payments.CountAsync(ct),
            CouponsCount = await _db.Coupons.CountAsync(ct),
            ReviewsCount = await _db.CourseReviews.CountAsync(ct),
            CertificatesCount = await _db.Certificates.CountAsync(ct),
            LiveSessionsCount = await _db.LiveSessions.CountAsync(ct),
            NotificationsCount = await _db.Notifications.CountAsync(ct),

            MyCoursesCount = await _db.Courses.CountAsync(c => c.TeacherId == userId, ct),
            MyEnrollmentsCount = await _db.Enrollments.CountAsync(e => e.StudentId == userId, ct),
            MyWishlistCount = await _db.WishlistItems.CountAsync(w => w.UserId == userId, ct),
            MyCartCount = await _db.CartItems.CountAsync(c => c.UserId == userId, ct),
            MyOrdersCount = await _db.Orders.CountAsync(o => o.UserId == userId, ct),
            MyCertificatesCount = await _db.Certificates.CountAsync(c => c.StudentId == userId, ct),
            MyUnreadNotificationsCount = await _db.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead, ct),

            TotalRevenue = await _db.Orders
                .Where(o => o.Status == OrderStatus.Paid)
                .SumAsync(o => (decimal?)o.Total, ct) ?? 0,

            MyRevenue = await _db.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Course)
                .Where(o => o.Status == OrderStatus.Paid &&
                            o.Items.Any(i => i.Course != null && i.Course.TeacherId == userId))
                .SumAsync(o => (decimal?)o.Total, ct) ?? 0,

            RecentCourses = recentCourses,
            RecentEnrollments = recentEnrollments,
            RecentOrders = recentOrders,
            RecentNotifications = notifications,
            UpcomingLiveSessions = upcomingSessions
        };

        return View(vm);
    }
}
