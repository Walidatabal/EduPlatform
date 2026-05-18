using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

public class Subject : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int GradeId { get; set; }
    public Grade? Grade { get; set; }
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
