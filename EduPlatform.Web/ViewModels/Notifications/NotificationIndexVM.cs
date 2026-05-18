namespace EduPlatform.Web.ViewModels.Notifications;

/// <summary>
/// Notifications page ViewModel.
/// </summary>
public class NotificationIndexVM
{
    public List<NotificationItemVM> Notifications { get; set; } = [];
}

public class NotificationItemVM
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Url { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }
}
