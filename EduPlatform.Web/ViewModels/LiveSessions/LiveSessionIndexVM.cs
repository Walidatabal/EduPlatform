namespace EduPlatform.Web.ViewModels.LiveSessions;

/// <summary>
/// Live sessions page ViewModel.
/// </summary>
public class LiveSessionIndexVM
{
    public List<LiveSessionItemVM> Sessions { get; set; } = [];
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
}
