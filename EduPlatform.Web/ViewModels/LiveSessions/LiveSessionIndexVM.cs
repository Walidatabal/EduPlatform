namespace EduPlatform.Web.ViewModels.LiveSessions;

public class LiveSessionIndexVM
{
    public List<LiveSessionItemVM> Sessions { get; set; } = [];
    public bool CanManageSessions { get; set; }
}

public class LiveSessionItemVM
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? MeetingUrl { get; set; }
    public bool IsInstructor { get; set; }
    public bool IsRegistered { get; set; }
    public bool HasSignedIn { get; set; }

    public bool IsLive => Status == "Live";
    public bool IsScheduled => Status == "Scheduled";
    public bool CanGoLive => IsScheduled && IsInstructor;
    public bool CanComplete => IsLive && IsInstructor;

    // Student can sign in if: session is Live, they are registered, and haven't signed in yet
    public bool CanSignIn => IsLive && IsRegistered && !HasSignedIn;

    // Student can sign out if: session is Live and they have signed in
    public bool CanSignOut => IsLive && HasSignedIn;
}

public class LiveSessionAttendanceVM
{
    public int SessionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<AttendeeRowVM> Attendees { get; set; } = [];

    public int PresentCount => Attendees.Count(a => a.Status is "Present" or "Late");
    public int AbsentCount => Attendees.Count(a => a.Status == "Absent");
    public int LateCount => Attendees.Count(a => a.Status == "Late");
    public int RegisteredCount => Attendees.Count(a => a.Status == "Registered");
}

public class AttendeeRowVM
{
    public int AttendanceId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public int? DurationMinutes { get; set; }
    public string? Note { get; set; }

    public string StatusBadgeClass => Status switch
    {
        "Present" => "green",
        "Late" => "amber",
        "Absent" => "red",
        "Excused" => "blue",
        "Registered" => "gray",
        _ => "gray"
    };
}

public class MyAttendanceVM
{
    public List<MyAttendanceRowVM> Records { get; set; } = [];

    public int TotalSessions => Records.Count;
    public int PresentCount => Records.Count(r => r.Status is "Present" or "Late");
    public int AbsentCount => Records.Count(r => r.Status == "Absent");
    public int AttendanceRate => TotalSessions == 0 ? 0
        : (int)Math.Round((double)PresentCount / TotalSessions * 100);
}

public class MyAttendanceRowVM
{
    public int SessionId { get; set; }
    public string SessionTitle { get; set; } = string.Empty;
    public string CourseTitle { get; set; } = string.Empty;
    public DateTime SessionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? JoinedAt { get; set; }
    public DateTime? LeftAt { get; set; }
    public int? DurationMinutes { get; set; }
}