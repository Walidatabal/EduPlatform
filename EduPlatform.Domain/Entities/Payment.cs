using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

public class Payment : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? GatewayTransactionId { get; set; }
    public string Gateway { get; set; } = string.Empty;
}
