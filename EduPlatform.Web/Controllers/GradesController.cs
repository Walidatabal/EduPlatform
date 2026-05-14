using EduPlatform.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Web.Controllers;

public class GradesController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public GradesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var grades = await _unitOfWork.Grades.GetAllAsync(ct);
        return View(grades);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var grade = await _unitOfWork.Grades.GetWithSubjectsAsync(id, ct);
        if (grade is null)
        {
            return NotFound();
        }

        return View(grade);
    }
}
