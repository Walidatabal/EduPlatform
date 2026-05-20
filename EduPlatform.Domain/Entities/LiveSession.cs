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

    public ICollection<SessionAttendance> Attendances { get; set; } = [];

    public void GoLive()
    {
        if (Status != LiveSessionStatus.Scheduled)
            throw new InvalidOperationException($"Cannot go live from status '{Status}'.");
        Status = LiveSessionStatus.Live;
    }

    public void Complete()
    {
        if (Status != LiveSessionStatus.Live)
            throw new InvalidOperationException($"Cannot complete a session in status '{Status}'.");
        Status = LiveSessionStatus.Completed;
        foreach (var att in Attendances.Where(a => a.Status == AttendanceStatus.Registered))
            att.Status = AttendanceStatus.Absent;
    }

    public void Cancel()
    {
        if (Status == LiveSessionStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed session.");
        Status = LiveSessionStatus.Cancelled;
    }
}
