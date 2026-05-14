using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

public class CourseReviewService : ICourseReviewService
{
    private readonly ILmsPlatformService _lms;

    public CourseReviewService(ILmsPlatformService lms) => _lms = lms;

    public Task<IReadOnlyList<CourseReviewDto>> GetCourseReviewsAsync(int courseId, CancellationToken ct = default) => _lms.GetCourseReviewsAsync(courseId, ct);
    public Task<CourseReviewDto> AddOrUpdateReviewAsync(string studentId, int courseId, UpsertCourseReviewRequest request, CancellationToken ct = default) => _lms.AddOrUpdateReviewAsync(studentId, courseId, request, ct);
    public Task DeleteReviewAsync(int reviewId, string userId, bool isAdmin, CancellationToken ct = default) => _lms.DeleteReviewAsync(reviewId, userId, isAdmin, ct);
}
