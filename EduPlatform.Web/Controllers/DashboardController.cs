using EduPlatform.Domain.Interfaces;
using EduPlatform.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = new DashboardVM
        {
            GradesCount = await _unitOfWork.Grades.CountAsync(ct: ct),
            SubjectsCount = await _unitOfWork.Subjects.CountAsync(ct: ct),
            CoursesCount = await _unitOfWork.Courses.CountAsync(ct: ct),
            EnrollmentsCount = await _unitOfWork.Enrollments.CountAsync(ct: ct)
        };

        return View(vm);
    }
}
