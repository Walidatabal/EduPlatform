namespace EduPlatform.Web.ViewModels.Cart;

/// <summary>
/// Main cart page ViewModel.
///
/// Why ViewModel?
/// - Designed specifically for UI rendering
/// - Keeps UI separated from DTOs/entities
/// - Allows future UI-specific properties
/// - Prevents exposing backend contracts directly to views
/// </summary>
public class CartIndexVM
{
    /// <summary>
    /// Cart items displayed in UI table/cards.
    /// </summary>
    public List<CartItemVM> Items { get; set; } = [];

    /// <summary>
    /// Total cart amount shown in checkout summary.
    /// </summary>
    public decimal TotalAmount { get; set; }
}

/// <summary>
/// Represents a single cart item inside UI.
/// </summary>
public class CartItemVM
{
    /// <summary>
    /// Cart item database identifier.
    /// </summary>
    // Needed for:
    // - remove operations
    // - future quantity updates
    // - tracking individual cart rows
    public int Id { get; set; }

    /// <summary>
    /// Course identifier.
    /// </summary>
    public int CourseId { get; set; }

    /// <summary>
    /// Course title displayed in UI.
    /// </summary>
    public string CourseTitle { get; set; } = string.Empty;

    /// <summary>
    /// Price snapshot at time of adding to cart.
    /// </summary>
    public decimal PriceSnapshot { get; set; }
}