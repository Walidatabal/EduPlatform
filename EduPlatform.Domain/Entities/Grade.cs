using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Entities;

public class Grade : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
