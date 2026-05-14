namespace EduPlatform.Application.Features.Grades.DTOs;

public class GradeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SubjectCount { get; set; }
}
