using EduPlatform.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Web.Controllers;

public class HomeController : Controller
{
    private readonly IUnitOfWork _uow;
    public HomeController(IUnitOfWork uow) { _uow = uow; }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        ViewBag.CourseCount    = await _uow.Courses.CountAsync(ct: ct);
        ViewBag.GradeCount     = await _uow.Grades.CountAsync(ct: ct);
        ViewBag.EnrollCount    = await _uow.Enrollments.CountAsync(ct: ct);
        return View();
    }

    public IActionResult Error() => View();
}
