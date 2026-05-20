using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Web.ViewModels.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// MVC controller for student wishlist operations.
/// </summary>
[Authorize]
public class WishlistController : Controller
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = GetCurrentUserId();

        var items = await _wishlistService.GetWishlistAsync(userId, ct);

        var vm = new WishlistIndexVM
        {
            Items = items.Select(x => new WishlistItemVM
            {
                Id = x.Id,
                CourseId = x.CourseId,
                CourseTitle = x.CourseTitle,
                ThumbnailUrl = x.ThumbnailUrl,
                Price = x.Price
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int courseId, CancellationToken ct)
    {
        await _wishlistService.AddToWishlistAsync(GetCurrentUserId(), courseId, ct);
        TempData["Success"] = "Course added to wishlist.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int courseId, CancellationToken ct)
    {
        await _wishlistService.RemoveFromWishlistAsync(GetCurrentUserId(), courseId, ct);
        TempData["Success"] = "Course removed from wishlist.";
        return RedirectToAction(nameof(Index));
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }
}
