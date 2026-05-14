using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

public class CourseAnswer : BaseEntity
{
    public int CourseQuestionId { get; set; }
    public CourseQuestion? CourseQuestion { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public bool IsInstructorAnswer { get; set; }
}
