using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

/// <summary>
/// Represents a purchase order created at checkout.
///
/// Order lifecycle:
/// 1. Student clicks "Place Order" → Order is created with Status = Pending.
/// 2. Payment gateway processes the payment → Status = Paid.
/// 3. On payment failure → Status = Failed (order kept for audit trail).
/// 4. On refund → Status = Refunded (future: handled by payment gateway webhook).
///
/// Financial snapshot design:
/// An Order stores three financial values independently so the financial
/// record is immutable after creation:
/// - <see cref="Subtotal"/>: sum of all OrderItem prices before discount.
/// - <see cref="DiscountAmount"/>: coupon discount applied.
/// - <see cref="Total"/> = Subtotal − DiscountAmount; what the student was charged.
///
/// These values never change after the order is created. If a teacher later
/// changes a course price, that does not affect existing orders.
///
/// The <see cref="CouponCode"/> stores which coupon was used for display and
/// audit purposes. The actual discount calculation lives in CouponService.
/// </summary>
public class Order : BaseEntity
{
    /// <summary>
    /// Identity GUID of the user who placed this order.
    /// Stored as string to keep Domain independent of ASP.NET Core Identity.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Sum of all OrderItem prices before any discount.
    /// Calculated at checkout from CartItem.PriceSnapshot values.
    /// Stored here so the receipt is accurate even if course prices change later.
    /// </summary>
    public decimal Subtotal { get; set; }

    /// <summary>
    /// Discount amount applied via coupon.
    /// 0 if no coupon was used.
    /// For percentage coupons: DiscountAmount = Subtotal × (DiscountPercent / 100).
    /// For fixed-amount coupons: DiscountAmount = CouponFixedValue (capped at Subtotal).
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Final charged amount = Subtotal − DiscountAmount.
    /// This is what the payment gateway is instructed to charge.
    /// Never negative — OrderService clamps to 0 if coupon exceeds subtotal.
    /// </summary>
    public decimal Total { get; set; }

    /// <summary>
    /// Coupon code applied to this order, if any.
    /// Stored for display and audit — the discount is already reflected in DiscountAmount.
    /// Null if no coupon was used.
    /// </summary>
    public string? CouponCode { get; set; }

    /// <summary>
    /// Current payment status of the order.
    /// Default is Pending — changed to Paid when the payment gateway confirms payment.
    /// </summary>
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    /// <summary>
    /// Line items: one OrderItem per course in this order.
    /// Each OrderItem stores its own price snapshot for immutable receipts.
    /// </summary>
    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
}
