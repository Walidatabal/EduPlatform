using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public decimal Price { get; set; }
}
