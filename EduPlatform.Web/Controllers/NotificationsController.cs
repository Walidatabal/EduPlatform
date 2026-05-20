using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Web.ViewModels.Notifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// MVC controller for user notifications.
/// </summary>
[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var items = await _notificationService.GetNotificationsAsync(GetCurrentUserId(), ct);

        var vm = new NotificationIndexVM
        {
            Notifications = items.Select(n => new NotificationItemVM
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                Url = n.Url,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id, CancellationToken ct)
    {
        await _notificationService.MarkNotificationReadAsync(GetCurrentUserId(), id, ct);
        TempData["Success"] = "Notification marked as read.";
        return RedirectToAction(nameof(Index));
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }
}
