using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface ILmsPlatformService
{
    Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default);
    Task<CategoryDto> CreateCategoryAsync(UpsertCategoryRequest request, CancellationToken ct = default);
    Task UpdateCategoryAsync(int id, UpsertCategoryRequest request, CancellationToken ct = default);
    Task DeleteCategoryAsync(int id, CancellationToken ct = default);

    Task<CartSummaryDto> GetCartAsync(string userId, CancellationToken ct = default);
    Task AddToCartAsync(string userId, int courseId, CancellationToken ct = default);
    Task RemoveFromCartAsync(string userId, int courseId, CancellationToken ct = default);
    Task ClearCartAsync(string userId, CancellationToken ct = default);

    Task<IReadOnlyList<WishlistItemDto>> GetWishlistAsync(string userId, CancellationToken ct = default);
    Task AddToWishlistAsync(string userId, int courseId, CancellationToken ct = default);
    Task RemoveFromWishlistAsync(string userId, int courseId, CancellationToken ct = default);

    Task<CouponDto> CreateCouponAsync(UpsertCouponRequest request, CancellationToken ct = default);
    Task<CouponValidationDto> ValidateCouponAsync(string code, decimal subtotal, CancellationToken ct = default);

    Task<IReadOnlyList<CourseReviewDto>> GetCourseReviewsAsync(int courseId, CancellationToken ct = default);
    Task<CourseReviewDto> AddOrUpdateReviewAsync(string studentId, int courseId, UpsertCourseReviewRequest request, CancellationToken ct = default);
    Task DeleteReviewAsync(int reviewId, string userId, bool isAdmin, CancellationToken ct = default);

    Task<IReadOnlyList<LiveSessionDto>> GetCourseLiveSessionsAsync(int courseId, CancellationToken ct = default);
    Task<string?> GetLiveSessionJoinUrlAsync(string userId, int sessionId, CancellationToken ct = default);
    Task<LiveSessionDto> CreateLiveSessionAsync(string instructorId, UpsertLiveSessionRequest request, CancellationToken ct = default);
    Task UpdateLiveSessionAsync(string userId, bool isAdmin, int id, UpsertLiveSessionRequest request, CancellationToken ct = default);
    Task<LiveSessionStatusDto> GoLiveAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default);
    Task<LiveSessionStatusDto> CompleteSessionAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default);
    Task CancelSessionAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default);
    Task<AttendanceDto> CheckInAsync(string studentId, int sessionId, CancellationToken ct = default);
    Task<AttendanceDto> CheckOutAsync(string studentId, int sessionId, CancellationToken ct = default);
    Task<SessionAttendanceSummaryDto> GetAttendanceSummaryAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default);
    Task<AttendanceDto> UpdateAttendanceAsync(string userId, bool isAdmin, int attendanceId, UpdateAttendanceRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<AttendanceDto>> GetMyAttendanceAsync(string studentId, CancellationToken ct = default);

    Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(string userId, CancellationToken ct = default);
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request, CancellationToken ct = default);
    Task MarkNotificationReadAsync(string userId, int id, CancellationToken ct = default);

    Task<IReadOnlyList<OrderDto>> GetOrdersAsync(string userId, CancellationToken ct = default);
    Task<OrderDto> CheckoutAsync(string userId, CheckoutRequest request, CancellationToken ct = default);

    Task CompleteLessonAsync(string studentId, int lessonId, CompleteLessonRequest request, CancellationToken ct = default);
    Task<ProgressDto> GetCourseProgressAsync(string studentId, int courseId, CancellationToken ct = default);

    Task<IReadOnlyList<CertificateDto>> GetCertificatesAsync(string studentId, CancellationToken ct = default);
    Task<CertificateDto> IssueCertificateAsync(string studentId, int courseId, CancellationToken ct = default);
    Task RevokeCertificateAsync(int id, CancellationToken ct = default);

    Task<IReadOnlyList<QuestionDto>> GetCourseQuestionsAsync(int courseId, CancellationToken ct = default);
    Task<QuestionDto> AskQuestionAsync(string studentId, int courseId, AskQuestionRequest request, CancellationToken ct = default);
    Task<AnswerDto> AnswerQuestionAsync(string userId, bool isAdmin, int courseId, int questionId, AnswerQuestionRequest request, CancellationToken ct = default);

    Task<SectionManagementDto> CreateSectionAsync(string userId, bool isAdmin, int courseId, UpsertSectionRequest request, CancellationToken ct = default);
    Task UpdateSectionAsync(string userId, bool isAdmin, int sectionId, UpsertSectionRequest request, CancellationToken ct = default);
    Task DeleteSectionAsync(string userId, bool isAdmin, int sectionId, CancellationToken ct = default);

    Task<LessonManagementDto> CreateLessonAsync(string userId, bool isAdmin, int sectionId, UpsertLessonRequest request, CancellationToken ct = default);
    Task UpdateLessonAsync(string userId, bool isAdmin, int lessonId, UpsertLessonRequest request, CancellationToken ct = default);
    Task DeleteLessonAsync(string userId, bool isAdmin, int lessonId, CancellationToken ct = default);
    Task ReorderLessonsAsync(string userId, bool isAdmin, int sectionId, ReorderLessonsRequest request, CancellationToken ct = default);
}
