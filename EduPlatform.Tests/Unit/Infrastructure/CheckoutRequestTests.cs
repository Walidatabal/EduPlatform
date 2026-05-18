using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Tests.Unit.Infrastructure;

/// <summary>
/// Unit tests for the CheckoutRequest record.
///
/// What is CheckoutRequest?
/// CheckoutRequest is the input model for the checkout endpoint.
/// It carries the optional coupon code the student wants to apply.
/// The coupon code is optional — most checkouts proceed without one.
///
/// These tests verify the constructor behavior of the C# record type.
/// They are simple but important: the checkout flow is critical business
/// logic and even a null-handling bug in the request model could
/// silently cause checkout failures.
/// </summary>
public class CheckoutRequestTests
{
    /// <summary>
    /// A checkout without a coupon must create a valid request with CouponCode = null.
    /// The checkout flow must NOT throw when CouponCode is null.
    /// OrderService must handle null coupon gracefully (skip coupon validation).
    /// </summary>
    [Fact]
    public void CheckoutRequest_NullCoupon_IsAllowed()
    {
        var req = new CheckoutRequest(null);

        Assert.Null(req.CouponCode);
    }

    /// <summary>
    /// When a student provides a coupon code, it must be stored exactly as provided.
    /// The service will validate the code against the Coupons table.
    /// Case sensitivity depends on the database collation — the request stores
    /// the code as-is without transformation.
    /// </summary>
    [Fact]
    public void CheckoutRequest_WithCoupon_StoresCouponCode()
    {
        var req = new CheckoutRequest("WELCOME10");

        Assert.Equal("WELCOME10", req.CouponCode);
    }
}
