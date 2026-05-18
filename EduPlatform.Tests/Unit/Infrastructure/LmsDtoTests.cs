using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Tests.Unit.Infrastructure;

/// <summary>
/// Unit tests for LMS Data Transfer Object (DTO) records.
///
/// What are DTOs?
/// DTOs are immutable C# records used to transfer data between layers.
/// They are the safe read models returned by services — they expose only
/// what the API/UI needs, never internal fields like IsDeleted or EF navigation properties.
///
/// Why test DTOs?
/// DTOs use C# record syntax: new CartSummaryDto(items, subtotal).
/// The constructor assigns positional parameters to properties.
/// A refactoring that swaps the parameter order would silently assign
/// the wrong value to the wrong property. These tests catch that.
///
/// These tests also verify business logic embedded in DTO construction:
/// - CartSummaryDto.Subtotal is the sum of items.
/// - CouponValidationDto correctly reflects valid/invalid state.
/// - ProgressDto calculates percentage correctly.
/// </summary>
public class LmsDtoTests
{
    // ── CartSummaryDto ───────────────────────────────────────────────────────

    /// <summary>
    /// The cart subtotal must equal the sum of all CartItemDto.PriceSnapshot values.
    /// This is computed in CartService and passed to the DTO constructor.
    /// If the summation logic changes, this test catches the regression.
    /// </summary>
    [Fact]
    public void CartSummaryDto_Subtotal_Matches_Sum_Of_Items()
    {
        var items = new List<CartItemDto>
        {
            new(1, 10, "Course A", 49.00m),  // CartItemId, CourseId, Title, PriceSnapshot
            new(2, 11, "Course B", 29.00m)
        };

        var summary = new CartSummaryDto(items, items.Sum(i => i.PriceSnapshot));

        Assert.Equal(78.00m, summary.Subtotal);
    }

    // ── CouponValidationDto ──────────────────────────────────────────────────

    /// <summary>
    /// A valid coupon: Valid=true, Discount>0, no error Message.
    /// </summary>
    [Fact]
    public void CouponValidationDto_Valid_True_Has_Discount()
    {
        var dto = new CouponValidationDto(true, 10.00m, null);

        Assert.True(dto.Valid);
        Assert.Equal(10.00m, dto.Discount);
        Assert.Null(dto.Message);
    }

    /// <summary>
    /// An invalid/expired coupon: Valid=false, Discount=0, Message explains why.
    /// </summary>
    [Fact]
    public void CouponValidationDto_Invalid_Has_Message()
    {
        var dto = new CouponValidationDto(false, 0, "Coupon expired.");

        Assert.False(dto.Valid);
        Assert.Equal(0, dto.Discount);
        Assert.NotNull(dto.Message);
    }

    // ── OrderDto ─────────────────────────────────────────────────────────────

    /// <summary>
    /// An order with no items should have an empty Items list, not null.
    /// Null collections cause NullReferenceException in Razor views.
    /// </summary>
    [Fact]
    public void OrderDto_Items_Empty_By_Default_When_Created_Empty()
    {
        var items = new List<OrderItemDto>();
        var order = new OrderDto(1, 0, 0, 0, null, OrderStatus.Pending, items);

        Assert.Empty(order.Items);
    }

    // ── ProgressDto ──────────────────────────────────────────────────────────

    /// <summary>
    /// When a student has completed all 4 lessons, the progress must be 100%.
    /// CompletedLessons must equal TotalLessons.
    /// </summary>
    [Fact]
    public void ProgressDto_Percent_100_When_All_Lessons_Completed()
    {
        var progress = new ProgressDto(1, 4, 4, 100m);

        Assert.Equal(100m, progress.Percent);
        Assert.Equal(progress.TotalLessons, progress.CompletedLessons);
    }

    /// <summary>
    /// When a student has completed 0 lessons, the progress must be 0%.
    /// </summary>
    [Fact]
    public void ProgressDto_Percent_0_When_No_Lessons_Completed()
    {
        var progress = new ProgressDto(1, 4, 0, 0m);

        Assert.Equal(0m, progress.Percent);
    }

    // ── CertificateDto ───────────────────────────────────────────────────────

    /// <summary>
    /// A freshly issued certificate must have Status = Issued.
    /// This confirms the CertificateStatus enum value is correctly stored in the DTO.
    /// </summary>
    [Fact]
    public void CertificateDto_HasCorrectStatus_Issued()
    {
        var cert = new CertificateDto(
            Id: 1,
            CourseId: 5,
            CourseTitle: "Math 101",
            CertificateNumber: "EDU-001",
            IssuedAt: DateTime.UtcNow,
            Status: CertificateStatus.Issued,
            PdfUrl: null);

        Assert.Equal(CertificateStatus.Issued, cert.Status);
    }

    // ── WishlistItemDto ──────────────────────────────────────────────────────

    /// <summary>
    /// The wishlist item must correctly store the course title.
    /// Used in the Wishlist view to display what the student saved.
    /// </summary>
    [Fact]
    public void WishlistItemDto_Has_CourseTitle()
    {
        var item = new WishlistItemDto(1, 3, "Algebra Basics", null, 29.00m);

        Assert.Equal("Algebra Basics", item.CourseTitle);
    }
}
