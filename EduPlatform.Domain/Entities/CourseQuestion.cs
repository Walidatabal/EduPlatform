using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

public class CourseQuestion : BaseEntity
{
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public QuestionStatus Status { get; set; } = QuestionStatus.Open;
    public ICollection<CourseAnswer> Answers { get; set; } = new List<CourseAnswer>();
}
