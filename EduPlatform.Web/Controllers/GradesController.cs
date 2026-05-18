using EduPlatform.Domain.Interfaces;
using EduPlatform.Web.ViewModels.Grades;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Web.Controllers;

public class GradesController : Controller
{
    private readonly IUnitOfWork _uow;
    public GradesController(IUnitOfWork uow) { _uow = uow; }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var grades = await _uow.Grades.GetAllAsync(ct);
        var vm = new List<GradeListItemVM>();
        foreach (var g in grades)
        {
            var withSubjects = await _uow.Grades.GetWithSubjectsAsync(g.Id, ct);
            vm.Add(new GradeListItemVM
            {
                Id           = g.Id,
                Name         = g.Name,
                Description  = g.Description,
                SubjectCount = withSubjects?.Subjects.Count ?? 0,
                CourseCount  = withSubjects?.Subjects.Sum(s => s.Courses.Count) ?? 0
            });
        }
        return View(vm);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var grade = await _uow.Grades.GetWithSubjectsAsync(id, ct);
        if (grade is null) return NotFound();

        var vm = new GradeDetailsVM
        {
            Id          = grade.Id,
            Name        = grade.Name,
            Description = grade.Description,
            Subjects    = grade.Subjects.Select(s => new SubjectItemVM
            {
                Id          = s.Id,
                Name        = s.Name,
                Description = s.Description,
                CourseCount = s.Courses.Count
            }).ToList()
        };
        return View(vm);
    }
}
