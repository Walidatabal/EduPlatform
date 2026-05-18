using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

public class LiveSession : BaseEntity
{
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public string InstructorId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? MeetingUrl { get; set; }
    public LiveSessionStatus Status { get; set; } = LiveSessionStatus.Scheduled;
    public int? MaxStudents { get; set; }
    public bool IsRecorded { get; set; }
    public string? RecordingUrl { get; set; }
}
