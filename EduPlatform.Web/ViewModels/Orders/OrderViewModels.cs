namespace EduPlatform.Web.ViewModels.Orders;

/// <summary>
/// Orders listing ViewModel.
/// </summary>
public class OrdersIndexVM
{
    public List<OrderItemVM> Orders { get; set; } = [];
}

public class OrderItemVM
{
    public int Id { get; set; }
    public decimal Subtotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    public string? CouponCode { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<OrderCourseVM> Items { get; set; } = [];
}

public class OrderCourseVM
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

/// <summary>
/// Checkout ViewModel used by the checkout form.
/// </summary>
public class CheckoutVM
{
    public decimal Subtotal { get; set; }
    public string? CouponCode { get; set; }
    public List<OrderCourseVM> Items { get; set; } = [];
}
