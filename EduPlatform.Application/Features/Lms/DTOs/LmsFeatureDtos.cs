using EduPlatform.Domain.Enums;

namespace EduPlatform.Application.Features.Lms.DTOs;

public record CategoryDto(int Id, string Name, string? Description, string? IconUrl, int? ParentCategoryId);
public record UpsertCategoryRequest(string Name, string? Description, string? IconUrl, int? ParentCategoryId);

public record CartItemDto(int Id, int CourseId, string CourseTitle, decimal PriceSnapshot);
public record CartSummaryDto(IReadOnlyList<CartItemDto> Items, decimal Subtotal);
public record WishlistItemDto(int Id, int CourseId, string CourseTitle, string? ThumbnailUrl, decimal Price);

public record UpsertCouponRequest(string Code, CouponDiscountType DiscountType, decimal DiscountValue, DateTime? StartsAt, DateTime? ExpiresAt, int? MaxUses);
public record CouponDto(int Id, string Code, CouponDiscountType DiscountType, decimal DiscountValue, DateTime? StartsAt, DateTime? ExpiresAt, bool IsActive, int UsedCount, int? MaxUses);
public record CouponValidationDto(bool Valid, decimal Discount, string? Message);

public record CourseReviewDto(int Id, int CourseId, string StudentId, int Rating, string? Comment, bool IsApproved, DateTime CreatedAt);
public record UpsertCourseReviewRequest(int Rating, string? Comment);

public record LiveSessionDto(int Id, int CourseId, string Title, string? Description, DateTime StartTime, DateTime EndTime, string? MeetingUrl, LiveSessionStatus Status, bool IsRecorded, string? RecordingUrl);
public record UpsertLiveSessionRequest(int CourseId, string Title, string? Description, DateTime StartTime, DateTime EndTime, string? MeetingUrl, int? MaxStudents, bool IsRecorded, string? RecordingUrl);

public record NotificationDto(int Id, string UserId, string Title, string Message, string? Url, bool IsRead, DateTime CreatedAt, DateTime? ReadAt);
public record CreateNotificationRequest(string UserId, string Title, string Message, string? Url);

public record OrderItemDto(int CourseId, string CourseTitle, decimal Price);
public record OrderDto(int Id, decimal Subtotal, decimal DiscountAmount, decimal Total, string? CouponCode, OrderStatus Status, IReadOnlyList<OrderItemDto> Items);
public record CheckoutRequest(string? CouponCode);

public record ProgressDto(int CourseId, int TotalLessons, int CompletedLessons, decimal Percent);
public record CompleteLessonRequest(int WatchedSeconds);

public record CertificateDto(int Id, int CourseId, string CourseTitle, string CertificateNumber, DateTime IssuedAt, CertificateStatus Status, string? PdfUrl);

public record QuestionDto(int Id, string Title, string Body, string StudentId, QuestionStatus Status, IReadOnlyList<AnswerDto> Answers, DateTime CreatedAt);
public record AnswerDto(int Id, string UserId, string Body, bool IsInstructorAnswer, DateTime CreatedAt);
public record AskQuestionRequest(string Title, string Body);
public record AnswerQuestionRequest(string Body);

public record SectionManagementDto(int Id, int CourseId, string Title, int Order);
public record UpsertSectionRequest(string Title, int Order);
public record LessonManagementDto(int Id, int SectionId, string Title, string? VideoUrl, int DurationSeconds, int Order, bool IsFreePreview, string ContentType, string? ArticleHtml, string? ResourceUrl);
public record UpsertLessonRequest(string Title, string? VideoUrl, int DurationSeconds, int Order, bool IsFreePreview, string ContentType, string? ArticleHtml, string? ResourceUrl);
public record ReorderItemRequest(int Id, int Order);
public record ReorderLessonsRequest(IReadOnlyList<ReorderItemRequest> Items);
