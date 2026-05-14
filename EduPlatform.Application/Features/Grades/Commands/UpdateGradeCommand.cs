using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Application.Features.Grades.Commands;

public class UpdateGradeCommand
{
    [Required, MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(500)] public string? Description { get; set; }
}
