using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface ICourseReviewService
{
    Task<IReadOnlyList<CourseReviewDto>> GetCourseReviewsAsync(int courseId, CancellationToken ct = default);
    Task<CourseReviewDto> AddOrUpdateReviewAsync(string studentId, int courseId, UpsertCourseReviewRequest request, CancellationToken ct = default);
    Task DeleteReviewAsync(int reviewId, string userId, bool isAdmin, CancellationToken ct = default);
}
