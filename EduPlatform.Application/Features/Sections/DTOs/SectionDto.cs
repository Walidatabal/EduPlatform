using EduPlatform.Application.Features.Lessons.DTOs;

namespace EduPlatform.Application.Features.Sections.DTOs;

public class SectionDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Order { get; set; }
    public IList<LessonDto> Lessons { get; set; } = new List<LessonDto>();
}
