using EduPlatform.Infrastructure.Data;
using EduPlatform.Web.ViewModels.LiveSessions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// MVC controller for live sessions overview.
/// </summary>
[Authorize]
public class LiveSessionsController : Controller
{
    private readonly AppDbContext _db;

    public LiveSessionsController(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var vm = new LiveSessionIndexVM
        {
            Sessions = await _db.LiveSessions
                .AsNoTracking()
                .Include(l => l.Course)
                .OrderBy(l => l.StartTime)
                .Select(l => new LiveSessionItemVM
                {
                    Id = l.Id,
                    CourseId = l.CourseId,
                    CourseTitle = l.Course != null ? l.Course.Title : $"Course #{l.CourseId}",
                    Title = l.Title,
                    Description = l.Description,
                    StartTime = l.StartTime,
                    EndTime = l.EndTime,
                    Status = l.Status.ToString(),
                    MeetingUrl = l.MeetingUrl
                })
                .ToListAsync(ct)
        };

        return View(vm);
    }
}
