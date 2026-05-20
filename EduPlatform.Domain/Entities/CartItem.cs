using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

/// <summary>
/// Represents a single course in a user's shopping cart.
///
/// Key design decision — PriceSnapshot:
/// When a student adds a course to their cart, the current Course.Price is captured
/// in <see cref="PriceSnapshot"/>. If the teacher later changes the price from
/// 50 KD to 80 KD, the cart still displays 50 KD.
///
/// Why this matters:
/// - Prevents unexpected price increases after a student has decided to buy.
/// - Matches the behavior of every major e-commerce platform (Amazon, Udemy).
/// - In most jurisdictions, showing a price then charging more at checkout is illegal.
///
/// The same snapshot pattern is applied to <see cref="OrderItem"/> at checkout,
/// ensuring the order record always reflects what the student actually paid.
///
/// Relationship: one User has many CartItems (one per course they intend to buy).
/// A user cannot have two CartItems for the same course — enforced in CartService.
/// </summary>
public class CartItem : BaseEntity
{
    /// <summary>
    /// Identity GUID of the user who added this item to their cart.
    /// Stored as string to keep Domain independent of ASP.NET Core Identity.
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>Foreign key to the course being added to cart.</summary>
    public int CourseId { get; set; }

    /// <summary>Navigation property — loaded only when explicitly included.</summary>
    public Course? Course { get; set; }

    /// <summary>
    /// The course price captured at the moment the item was added to the cart.
    /// This value does NOT change if the teacher later modifies Course.Price.
    /// Used for cart total calculation and for displaying "original price" on checkout.
    /// </summary>
    public decimal PriceSnapshot { get; set; }
}
