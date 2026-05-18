using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

public class LessonProgress : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public int LessonId { get; set; }
    public Lesson? Lesson { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int WatchedSeconds { get; set; }
}
