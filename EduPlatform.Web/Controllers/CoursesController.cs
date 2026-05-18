using EduPlatform.Domain.Interfaces;
using EduPlatform.Infrastructure.Identity;
using EduPlatform.Web.ViewModels.Courses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Web.Controllers;

public class CoursesController : Controller
{
    private readonly IUnitOfWork                  _uow;
    private readonly UserManager<ApplicationUser> _users;

    public CoursesController(IUnitOfWork uow, UserManager<ApplicationUser> users)
    { _uow = uow; _users = users; }

    public async Task<IActionResult> Index(string? search, string? level, CancellationToken ct)
    {
        var courses = await _uow.Courses.GetPublishedAsync(ct);

        if (!string.IsNullOrWhiteSpace(search))
            courses = courses.Where(c =>
                c.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (c.Description ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

        if (!string.IsNullOrWhiteSpace(level))
            courses = courses.Where(c => c.Level == level).ToList();

        var vm = courses.Select(c => new CourseListItemVM
        {
            Id             = c.Id,
            Title          = c.Title,
            Description    = c.Description,
            Level          = c.Level,
            Language       = c.Language,
            Price          = c.Price,
            CategoryName   = c.Category?.Name,
            SubjectName    = c.Subject?.Name,
            GradeName      = c.Subject?.Grade?.Name,
            ReviewCount    = c.Reviews.Count,
            AverageRating  = c.Reviews.Any() ? c.Reviews.Average(r => r.Rating) : 0,
            EnrollmentCount= c.Enrollments.Count
        }).ToList();

        ViewBag.Search = search;
        ViewBag.Level  = level;
        return View(vm);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var course = await _uow.Courses.GetWithDetailsAsync(id, ct);
        if (course is null) return NotFound();

        bool isEnrolled = false;
        if (User.Identity?.IsAuthenticated == true)
        {
            var user = await _users.GetUserAsync(User);
            if (user is not null)
                isEnrolled = await _uow.Enrollments.IsEnrolledAsync(user.Id, id, ct);
        }

        var vm = new CourseDetailsVM
        {
            Id               = course.Id,
            Title            = course.Title,
            Description      = course.Description,
            Requirements     = course.Requirements,
            LearningOutcomes = course.LearningOutcomes,
            Level            = course.Level,
            Language         = course.Language,
            Price            = course.Price,
            CategoryName     = course.Category?.Name,
            SubjectName      = course.Subject?.Name,
            GradeName        = course.Subject?.Grade?.Name,
            Status           = course.Status.ToString(),
            AverageRating    = course.Reviews.Any() ? course.Reviews.Average(r => r.Rating) : 0,
            ReviewCount      = course.Reviews.Count,
            EnrollmentCount  = course.Enrollments.Count,
            IsEnrolled       = isEnrolled,
            Sections = course.Sections.OrderBy(s => s.Order).Select(s => new CourseSectionVM
            {
                Order   = s.Order,
                Title   = s.Title,
                Lessons = s.Lessons.OrderBy(l => l.Order).Select(l => new CourseLessonVM
                {
                    Order       = l.Order,
                    Title       = l.Title,
                    ContentType = l.ContentType
                }).ToList()
            }).ToList(),
            Reviews = course.Reviews.Select(r => new CourseReviewVM
            {
                StudentName = "Student",
                Rating      = r.Rating,
                Comment     = r.Comment
            }).ToList()
        };
        return View(vm);
    }
}
