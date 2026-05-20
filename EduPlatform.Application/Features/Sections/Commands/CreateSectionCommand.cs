using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Application.Features.Sections.Commands;

public class CreateSectionCommand
{
    [Required, MaxLength(150)] public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public int CourseId { get; set; }
}
