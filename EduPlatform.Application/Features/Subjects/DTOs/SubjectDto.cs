namespace EduPlatform.Application.Features.Subjects.DTOs;

public class SubjectDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int GradeId { get; set; }
    public string GradeName { get; set; } = string.Empty;
}
