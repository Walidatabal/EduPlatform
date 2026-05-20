using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

public class SessionAttendance : BaseEntity
{
    public int LiveSessionId { get; set; }
    public LiveSession? LiveSession { get; set; }

    public string StudentId { get; set; } = string.Empty;

    public AttendanceStatus Status { get; set; } = AttendanceStatus.Registered;

    public DateTime? JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Note { get; set; }
}
