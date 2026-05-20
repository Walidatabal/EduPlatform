namespace EduPlatform.Web.ViewModels.Dashboard;

/// <summary>
/// Role-aware dashboard ViewModel.
/// 
/// Enterprise reason:
/// The dashboard should not be one static admin screen.
/// It should adapt to Admin, Teacher, Student, Parent, and ContentManager users.
/// </summary>
public class DashboardVM
{
    public string UserFullName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string UserRole { get; set; } = string.Empty;

    // Platform counters used mainly by Admin/ContentManager dashboards.
    public int UsersCount { get; set; }
    public int StudentsCount { get; set; }
    public int TeachersCount { get; set; }
    public int GradesCount { get; set; }
    public int SubjectsCount { get; set; }
    public int CoursesCount { get; set; }
    public int PublishedCoursesCount { get; set; }
    public int PendingCoursesCount { get; set; }
    public int EnrollmentsCount { get; set; }
    public int OrdersCount { get; set; }
    public int PaymentsCount { get; set; }
    public int CouponsCount { get; set; }
    public int ReviewsCount { get; set; }
    public int CertificatesCount { get; set; }
    public int LiveSessionsCount { get; set; }
    public int NotificationsCount { get; set; }

    // User-specific counters used by Teacher/Student dashboards.
    public int MyCoursesCount { get; set; }
    public int MyEnrollmentsCount { get; set; }
    public int MyWishlistCount { get; set; }
    public int MyCartCount { get; set; }
    public int MyOrdersCount { get; set; }
    public int MyCertificatesCount { get; set; }
    public int MyUnreadNotificationsCount { get; set; }

    public decimal TotalRevenue { get; set; }
    public decimal MyRevenue { get; set; }

    public IReadOnlyList<RecentCourseItem> RecentCourses { get; set; } = [];
    public IReadOnlyList<RecentEnrollmentItem> RecentEnrollments { get; set; } = [];
    public IReadOnlyList<RecentOrderItem> RecentOrders { get; set; } = [];
    public IReadOnlyList<NotificationItem> RecentNotifications { get; set; } = [];
    public IReadOnlyList<LiveSessionItem> UpcomingLiveSessions { get; set; } = [];
}

public class RecentCourseItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ApprovalStatus { get; set; } = string.Empty;
}

public class RecentEnrollmentItem
{
    public string CourseTitle { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class RecentOrderItem
{
    public int Id { get; set; }
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class NotificationItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LiveSessionItem
{
    public int Id { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string Status { get; set; } = string.Empty;
}
