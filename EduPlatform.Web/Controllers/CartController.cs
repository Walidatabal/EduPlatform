using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Web.ViewModels.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// MVC controller responsible for cart pages and cart actions.
/// </summary>
[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    /// <summary>
    /// Inject cart service abstraction from Application layer.
    /// </summary>
    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Displays current user cart.
    /// </summary>
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = GetCurrentUserId();

        var cart = await _cartService.GetCartAsync(userId, ct);

        var vm = new CartIndexVM
        {
            // Map DTO subtotal into the ViewModel total amount.
            TotalAmount = cart.Subtotal,

            // Map cart item DTOs into UI-specific ViewModels.
            Items = cart.Items.Select(x => new CartItemVM
            {
                Id = x.Id,
                CourseId = x.CourseId,
                CourseTitle = x.CourseTitle,
                PriceSnapshot = x.PriceSnapshot
            }).ToList()
        };

        return View(vm);
    }

    /// <summary>
    /// Adds selected course to cart.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int courseId, CancellationToken ct)
    {
        var userId = GetCurrentUserId();

        await _cartService.AddToCartAsync(userId, courseId, ct);

        TempData["Success"] = "Course added to cart successfully.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Removes selected course from cart.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int courseId, CancellationToken ct)
    {
        var userId = GetCurrentUserId();

        await _cartService.RemoveFromCartAsync(userId, courseId, ct);

        TempData["Success"] = "Course removed from cart.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Clears all cart items for current user.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        var userId = GetCurrentUserId();

        await _cartService.ClearCartAsync(userId, ct);

        TempData["Success"] = "Cart cleared successfully.";

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Gets authenticated user id from claims.
    /// </summary>
    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }
}