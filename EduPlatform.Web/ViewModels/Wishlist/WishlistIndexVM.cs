namespace EduPlatform.Web.ViewModels.Wishlist;

/// <summary>
/// Wishlist page model.
/// Keeps UI separated from Lms DTOs/entities.
/// </summary>
public class WishlistIndexVM
{
    public List<WishlistItemVM> Items { get; set; } = [];
}

public class WishlistItemVM
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
}
