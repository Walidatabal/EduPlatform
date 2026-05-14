using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

public class CartItem : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public decimal PriceSnapshot { get; set; }
}
