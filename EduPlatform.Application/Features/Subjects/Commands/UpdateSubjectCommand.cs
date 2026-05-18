using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Application.Features.Subjects.Commands;

public class UpdateSubjectCommand
{
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
    [Required] public int GradeId { get; set; }
}
