using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Web.ViewModels.Orders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// MVC controller for orders and checkout flow.
/// </summary>
[Authorize]
public class OrdersController : Controller
{
    private readonly IOrderService _orderService;
    private readonly ICartService _cartService;

    public OrdersController(IOrderService orderService, ICartService cartService)
    {
        _orderService = orderService;
        _cartService = cartService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var orders = await _orderService.GetOrdersAsync(GetCurrentUserId(), ct);

        var vm = new OrdersIndexVM
        {
            Orders = orders.Select(o => new OrderItemVM
            {
                Id = o.Id,
                Subtotal = o.Subtotal,
                DiscountAmount = o.DiscountAmount,
                Total = o.Total,
                CouponCode = o.CouponCode,
                Status = o.Status.ToString(),
                Items = o.Items.Select(i => new OrderCourseVM
                {
                    CourseId = i.CourseId,
                    CourseTitle = i.CourseTitle,
                    Price = i.Price
                }).ToList()
            }).ToList()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Checkout(CancellationToken ct)
    {
        var cart = await _cartService.GetCartAsync(GetCurrentUserId(), ct);

        var vm = new CheckoutVM
        {
            Subtotal = cart.Subtotal,
            Items = cart.Items.Select(i => new OrderCourseVM
            {
                CourseId = i.CourseId,
                CourseTitle = i.CourseTitle,
                Price = i.PriceSnapshot
            }).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(CheckoutVM model, CancellationToken ct)
    {
        var order = await _orderService.CheckoutAsync(
            GetCurrentUserId(),
            new CheckoutRequest(model.CouponCode),
            ct);

        TempData["Success"] = $"Order #{order.Id} created successfully.";
        return RedirectToAction(nameof(Index));
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }
}
