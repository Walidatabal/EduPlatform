using EduPlatform.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Web.Controllers;

public class CoursesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public CoursesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var courses = await _unitOfWork.Courses.GetPublishedAsync(ct);
        return View(courses);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var course = await _unitOfWork.Courses.GetWithDetailsAsync(id, ct);
        if (course is null)
        {
            return NotFound();
        }

        return View(course);
    }
}
