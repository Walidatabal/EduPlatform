using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface IProgressService
{
    Task CompleteLessonAsync(string studentId, int lessonId, CompleteLessonRequest request, CancellationToken ct = default);
    Task<ProgressDto> GetCourseProgressAsync(string studentId, int courseId, CancellationToken ct = default);
}
