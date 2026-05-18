using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

public class ProgressService : IProgressService
{
    private readonly ILmsPlatformService _lms;

    public ProgressService(ILmsPlatformService lms) => _lms = lms;

    public Task CompleteLessonAsync(string studentId, int lessonId, CompleteLessonRequest request, CancellationToken ct = default) => _lms.CompleteLessonAsync(studentId, lessonId, request, ct);
    public Task<ProgressDto> GetCourseProgressAsync(string studentId, int courseId, CancellationToken ct = default) => _lms.GetCourseProgressAsync(studentId, courseId, ct);
}
