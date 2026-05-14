using EduPlatform.Application.Common.Exceptions;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Enums;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Services.Lms;

public class LmsPlatformService : ILmsPlatformService
{
    private readonly AppDbContext _db;

    public LmsPlatformService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<CategoryDto>> GetCategoriesAsync(CancellationToken ct = default) =>
        await _db.Categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Description, c.IconUrl, c.ParentCategoryId))
            .ToListAsync(ct);

    public async Task<CategoryDto> CreateCategoryAsync(UpsertCategoryRequest request, CancellationToken ct = default)
    {
        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            IconUrl = request.IconUrl,
            ParentCategoryId = request.ParentCategoryId
        };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return new CategoryDto(category.Id, category.Name, category.Description, category.IconUrl, category.ParentCategoryId);
    }

    public async Task UpdateCategoryAsync(int id, UpsertCategoryRequest request, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync([id], ct) ?? throw new NotFoundException(nameof(Category), id);
        category.Name = request.Name.Trim();
        category.Description = request.Description;
        category.IconUrl = request.IconUrl;
        category.ParentCategoryId = request.ParentCategoryId;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteCategoryAsync(int id, CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync([id], ct) ?? throw new NotFoundException(nameof(Category), id);
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<CartSummaryDto> GetCartAsync(string userId, CancellationToken ct = default)
    {
        var items = await _db.CartItems.Include(c => c.Course)
            .Where(c => c.UserId == userId)
            .Select(c => new CartItemDto(c.Id, c.CourseId, c.Course!.Title, c.PriceSnapshot))
            .ToListAsync(ct);
        return new CartSummaryDto(items, items.Sum(i => i.PriceSnapshot));
    }

    public async Task AddToCartAsync(string userId, int courseId, CancellationToken ct = default)
    {
        var course = await _db.Courses.FindAsync([courseId], ct) ?? throw new NotFoundException(nameof(Course), courseId);
        if (await _db.Enrollments.AnyAsync(e => e.StudentId == userId && e.CourseId == courseId, ct))
            throw new ValidationException(new Dictionary<string, string[]> { ["Course"] = ["Student is already enrolled in this course."] });
        if (!await _db.CartItems.AnyAsync(c => c.UserId == userId && c.CourseId == courseId, ct))
        {
            _db.CartItems.Add(new CartItem { UserId = userId, CourseId = courseId, PriceSnapshot = course.Price });
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveFromCartAsync(string userId, int courseId, CancellationToken ct = default)
    {
        var item = await _db.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.CourseId == courseId, ct)
            ?? throw new NotFoundException("Cart item was not found.");
        _db.CartItems.Remove(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ClearCartAsync(string userId, CancellationToken ct = default)
    {
        var items = await _db.CartItems.Where(c => c.UserId == userId).ToListAsync(ct);
        _db.CartItems.RemoveRange(items);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<WishlistItemDto>> GetWishlistAsync(string userId, CancellationToken ct = default) =>
        await _db.WishlistItems.Include(w => w.Course)
            .Where(w => w.UserId == userId)
            .Select(w => new WishlistItemDto(w.Id, w.CourseId, w.Course!.Title, w.Course.ThumbnailUrl, w.Course.Price))
            .ToListAsync(ct);

    public async Task AddToWishlistAsync(string userId, int courseId, CancellationToken ct = default)
    {
        if (!await _db.Courses.AnyAsync(c => c.Id == courseId, ct)) throw new NotFoundException(nameof(Course), courseId);
        if (!await _db.WishlistItems.AnyAsync(w => w.UserId == userId && w.CourseId == courseId, ct))
        {
            _db.WishlistItems.Add(new WishlistItem { UserId = userId, CourseId = courseId });
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task RemoveFromWishlistAsync(string userId, int courseId, CancellationToken ct = default)
    {
        var item = await _db.WishlistItems.FirstOrDefaultAsync(w => w.UserId == userId && w.CourseId == courseId, ct)
            ?? throw new NotFoundException("Wishlist item was not found.");
        _db.WishlistItems.Remove(item);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<CouponDto> CreateCouponAsync(UpsertCouponRequest request, CancellationToken ct = default)
    {
        var coupon = new Coupon
        {
            Code = request.Code.Trim().ToUpperInvariant(),
            DiscountType = request.DiscountType,
            DiscountValue = request.DiscountValue,
            StartsAt = request.StartsAt,
            ExpiresAt = request.ExpiresAt,
            MaxUses = request.MaxUses,
            IsActive = true
        };
        _db.Coupons.Add(coupon);
        await _db.SaveChangesAsync(ct);
        return MapCoupon(coupon);
    }

    public async Task<CouponValidationDto> ValidateCouponAsync(string code, decimal subtotal, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var coupon = await _db.Coupons.FirstOrDefaultAsync(c => c.Code == code.ToUpperInvariant() && c.IsActive, ct);
        if (coupon is null || coupon.StartsAt > now || coupon.ExpiresAt < now || (coupon.MaxUses.HasValue && coupon.UsedCount >= coupon.MaxUses.Value))
            return new CouponValidationDto(false, 0, "Coupon is invalid or expired.");

        var discount = coupon.DiscountType == CouponDiscountType.Percentage
            ? subtotal * coupon.DiscountValue / 100
            : coupon.DiscountValue;
        return new CouponValidationDto(true, Math.Min(discount, subtotal), null);
    }

    public async Task<IReadOnlyList<CourseReviewDto>> GetCourseReviewsAsync(int courseId, CancellationToken ct = default) =>
        await _db.CourseReviews
            .Where(r => r.CourseId == courseId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new CourseReviewDto(r.Id, r.CourseId, r.StudentId, r.Rating, r.Comment, r.IsApproved, r.CreatedAt))
            .ToListAsync(ct);

    public async Task<CourseReviewDto> AddOrUpdateReviewAsync(string studentId, int courseId, UpsertCourseReviewRequest request, CancellationToken ct = default)
    {
        if (request.Rating < 1 || request.Rating > 5)
            throw new ValidationException(new Dictionary<string, string[]> { ["Rating"] = ["Rating must be between 1 and 5."] });
        if (!await _db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct))
            throw new ForbiddenException("Only enrolled students can review this course.");

        var review = await _db.CourseReviews.FirstOrDefaultAsync(r => r.StudentId == studentId && r.CourseId == courseId, ct);
        if (review is null)
        {
            review = new CourseReview { StudentId = studentId, CourseId = courseId };
            _db.CourseReviews.Add(review);
        }
        review.Rating = request.Rating;
        review.Comment = request.Comment;
        review.IsApproved = true;
        await _db.SaveChangesAsync(ct);
        return new CourseReviewDto(review.Id, review.CourseId, review.StudentId, review.Rating, review.Comment, review.IsApproved, review.CreatedAt);
    }

    public async Task DeleteReviewAsync(int reviewId, string userId, bool isAdmin, CancellationToken ct = default)
    {
        var review = await _db.CourseReviews.FindAsync([reviewId], ct) ?? throw new NotFoundException(nameof(CourseReview), reviewId);
        if (!isAdmin && review.StudentId != userId) throw new ForbiddenException();
        _db.CourseReviews.Remove(review);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<LiveSessionDto>> GetCourseLiveSessionsAsync(int courseId, CancellationToken ct = default) =>
        await _db.LiveSessions
            .Where(l => l.CourseId == courseId)
            .OrderBy(l => l.StartTime)
            .Select(l => MapLiveSession(l))
            .ToListAsync(ct);

    public async Task<string?> GetLiveSessionJoinUrlAsync(string userId, int sessionId, CancellationToken ct = default)
    {
        var session = await _db.LiveSessions.FirstOrDefaultAsync(l => l.Id == sessionId, ct) ?? throw new NotFoundException(nameof(LiveSession), sessionId);
        var isEnrolled = await _db.Enrollments.AnyAsync(e => e.StudentId == userId && e.CourseId == session.CourseId, ct);
        if (!isEnrolled) throw new ForbiddenException("Only enrolled students can join this live session.");
        return session.MeetingUrl;
    }

    public async Task<LiveSessionDto> CreateLiveSessionAsync(string instructorId, UpsertLiveSessionRequest request, CancellationToken ct = default)
    {
        var course = await _db.Courses.FindAsync([request.CourseId], ct) ?? throw new NotFoundException(nameof(Course), request.CourseId);
        if (course.TeacherId != instructorId) throw new ForbiddenException("Only course teacher can create live sessions.");
        var session = new LiveSession
        {
            CourseId = request.CourseId,
            InstructorId = instructorId,
            Title = request.Title,
            Description = request.Description,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            MeetingUrl = request.MeetingUrl,
            MaxStudents = request.MaxStudents,
            IsRecorded = request.IsRecorded,
            RecordingUrl = request.RecordingUrl
        };
        _db.LiveSessions.Add(session);
        await _db.SaveChangesAsync(ct);
        return MapLiveSession(session);
    }

    public async Task UpdateLiveSessionAsync(string userId, bool isAdmin, int id, UpsertLiveSessionRequest request, CancellationToken ct = default)
    {
        var session = await _db.LiveSessions.Include(l => l.Course).FirstOrDefaultAsync(l => l.Id == id, ct) ?? throw new NotFoundException(nameof(LiveSession), id);
        if (!isAdmin && session.Course?.TeacherId != userId) throw new ForbiddenException();
        session.Title = request.Title;
        session.Description = request.Description;
        session.StartTime = request.StartTime;
        session.EndTime = request.EndTime;
        session.MeetingUrl = request.MeetingUrl;
        session.MaxStudents = request.MaxStudents;
        session.IsRecorded = request.IsRecorded;
        session.RecordingUrl = request.RecordingUrl;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(string userId, CancellationToken ct = default) =>
        await _db.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationDto(n.Id, n.UserId, n.Title, n.Message, n.Url, n.IsRead, n.CreatedAt, n.ReadAt))
            .ToListAsync(ct);

    public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request, CancellationToken ct = default)
    {
        var notification = new Notification { UserId = request.UserId, Title = request.Title, Message = request.Message, Url = request.Url };
        _db.Notifications.Add(notification);
        await _db.SaveChangesAsync(ct);
        return new NotificationDto(notification.Id, notification.UserId, notification.Title, notification.Message, notification.Url, notification.IsRead, notification.CreatedAt, notification.ReadAt);
    }

    public async Task MarkNotificationReadAsync(string userId, int id, CancellationToken ct = default)
    {
        var notification = await _db.Notifications.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId, ct) ?? throw new NotFoundException(nameof(Notification), id);
        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(string userId, CancellationToken ct = default) =>
        await _db.Orders.Include(o => o.Items).ThenInclude(i => i.Course)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => MapOrder(o))
            .ToListAsync(ct);

    public async Task<OrderDto> CheckoutAsync(string userId, CheckoutRequest request, CancellationToken ct = default)
    {
        var cart = await _db.CartItems.Include(c => c.Course).Where(c => c.UserId == userId).ToListAsync(ct);
        if (cart.Count == 0)
            throw new ValidationException(new Dictionary<string, string[]> { ["Cart"] = ["Cart is empty."] });

        var subtotal = cart.Sum(c => c.PriceSnapshot);
        decimal discount = 0;
        Coupon? coupon = null;
        if (!string.IsNullOrWhiteSpace(request.CouponCode))
        {
            var validation = await ValidateCouponAsync(request.CouponCode, subtotal, ct);
            if (!validation.Valid)
                throw new ValidationException(new Dictionary<string, string[]> { ["Coupon"] = [validation.Message ?? "Invalid coupon."] });
            coupon = await _db.Coupons.FirstAsync(c => c.Code == request.CouponCode.ToUpperInvariant(), ct);
            discount = validation.Discount;
            coupon.UsedCount++;
        }

        var order = new Order
        {
            UserId = userId,
            Subtotal = subtotal,
            DiscountAmount = discount,
            Total = subtotal - discount,
            CouponCode = coupon?.Code,
            Status = OrderStatus.Pending // Payment gateway placeholder. Never mark as Paid before gateway confirmation.
        };

        foreach (var item in cart)
        {
            order.Items.Add(new OrderItem { CourseId = item.CourseId, Price = item.PriceSnapshot });
        }

        _db.Orders.Add(order);
        _db.CartItems.RemoveRange(cart);
        await _db.SaveChangesAsync(ct);
        return MapOrder(order);
    }

    public async Task CompleteLessonAsync(string studentId, int lessonId, CompleteLessonRequest request, CancellationToken ct = default)
    {
        var lesson = await _db.Lessons.Include(l => l.Section).FirstOrDefaultAsync(l => l.Id == lessonId, ct);
        if (lesson?.Section is null) throw new NotFoundException(nameof(Lesson), lessonId);
        var courseId = lesson.Section.CourseId;
        if (!await _db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct))
            throw new ForbiddenException("Student is not enrolled in this course.");

        var progress = await _db.LessonProgresses.FirstOrDefaultAsync(p => p.StudentId == studentId && p.LessonId == lessonId, ct);
        if (progress is null)
        {
            progress = new LessonProgress { StudentId = studentId, CourseId = courseId, LessonId = lessonId };
            _db.LessonProgresses.Add(progress);
        }

        progress.WatchedSeconds = Math.Max(progress.WatchedSeconds, request.WatchedSeconds);
        progress.IsCompleted = true;
        progress.CompletedAt ??= DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<ProgressDto> GetCourseProgressAsync(string studentId, int courseId, CancellationToken ct = default)
    {
        var totalLessons = await _db.Lessons.CountAsync(l => l.Section != null && l.Section.CourseId == courseId, ct);
        var completedLessons = await _db.LessonProgresses.CountAsync(p => p.StudentId == studentId && p.CourseId == courseId && p.IsCompleted, ct);
        var percent = totalLessons == 0 ? 0 : Math.Round(completedLessons * 100m / totalLessons, 2);
        return new ProgressDto(courseId, totalLessons, completedLessons, percent);
    }

    public async Task<IReadOnlyList<CertificateDto>> GetCertificatesAsync(string studentId, CancellationToken ct = default) =>
        await _db.Certificates.Include(c => c.Course)
            .Where(c => c.StudentId == studentId)
            .Select(c => new CertificateDto(c.Id, c.CourseId, c.Course!.Title, c.CertificateNumber, c.IssuedAt, c.Status, c.PdfUrl))
            .ToListAsync(ct);

    public async Task<CertificateDto> IssueCertificateAsync(string studentId, int courseId, CancellationToken ct = default)
    {
        if (!await _db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct))
            throw new ForbiddenException("Student is not enrolled in this course.");
        var totalLessons = await _db.Lessons.CountAsync(l => l.Section != null && l.Section.CourseId == courseId, ct);
        var completedLessons = await _db.LessonProgresses.CountAsync(p => p.StudentId == studentId && p.CourseId == courseId && p.IsCompleted, ct);
        if (totalLessons == 0 || completedLessons < totalLessons)
            throw new ValidationException(new Dictionary<string, string[]> { ["Course"] = ["Course is not completed yet."] });
        var existing = await _db.Certificates.Include(c => c.Course).FirstOrDefaultAsync(c => c.StudentId == studentId && c.CourseId == courseId, ct);
        if (existing is not null)
            return new CertificateDto(existing.Id, existing.CourseId, existing.Course!.Title, existing.CertificateNumber, existing.IssuedAt, existing.Status, existing.PdfUrl);
        var cert = new Certificate
        {
            StudentId = studentId,
            CourseId = courseId,
            CertificateNumber = $"EDU-{courseId}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}",
            IssuedAt = DateTime.UtcNow
        };
        _db.Certificates.Add(cert);
        await _db.SaveChangesAsync(ct);
        var courseTitle = await _db.Courses.Where(c => c.Id == courseId).Select(c => c.Title).FirstAsync(ct);
        return new CertificateDto(cert.Id, cert.CourseId, courseTitle, cert.CertificateNumber, cert.IssuedAt, cert.Status, cert.PdfUrl);
    }

    public async Task RevokeCertificateAsync(int id, CancellationToken ct = default)
    {
        var cert = await _db.Certificates.FindAsync([id], ct) ?? throw new NotFoundException(nameof(Certificate), id);
        cert.Status = CertificateStatus.Revoked;
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<QuestionDto>> GetCourseQuestionsAsync(int courseId, CancellationToken ct = default) =>
        await _db.CourseQuestions.Where(q => q.CourseId == courseId)
            .Include(q => q.Answers)
            .OrderByDescending(q => q.CreatedAt)
            .Select(q => new QuestionDto(q.Id, q.Title, q.Body, q.StudentId, q.Status,
                q.Answers.Select(a => new AnswerDto(a.Id, a.UserId, a.Body, a.IsInstructorAnswer, a.CreatedAt)).ToList(), q.CreatedAt))
            .ToListAsync(ct);

    public async Task<QuestionDto> AskQuestionAsync(string studentId, int courseId, AskQuestionRequest request, CancellationToken ct = default)
    {
        if (!await _db.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct))
            throw new ForbiddenException("Only enrolled students can ask questions.");
        var question = new CourseQuestion { CourseId = courseId, StudentId = studentId, Title = request.Title, Body = request.Body };
        _db.CourseQuestions.Add(question);
        await _db.SaveChangesAsync(ct);
        return new QuestionDto(question.Id, question.Title, question.Body, question.StudentId, question.Status, new List<AnswerDto>(), question.CreatedAt);
    }

    public async Task<AnswerDto> AnswerQuestionAsync(string userId, bool isAdmin, int courseId, int questionId, AnswerQuestionRequest request, CancellationToken ct = default)
    {
        var question = await _db.CourseQuestions.Include(q => q.Course).FirstOrDefaultAsync(q => q.Id == questionId && q.CourseId == courseId, ct)
            ?? throw new NotFoundException(nameof(CourseQuestion), questionId);
        var isInstructor = question.Course?.TeacherId == userId || isAdmin;
        var isEnrolled = await _db.Enrollments.AnyAsync(e => e.StudentId == userId && e.CourseId == courseId, ct);
        if (!isInstructor && !isEnrolled) throw new ForbiddenException();
        var answer = new CourseAnswer { CourseQuestionId = questionId, UserId = userId, Body = request.Body, IsInstructorAnswer = isInstructor };
        if (isInstructor) question.Status = QuestionStatus.Answered;
        _db.CourseAnswers.Add(answer);
        await _db.SaveChangesAsync(ct);
        return new AnswerDto(answer.Id, answer.UserId, answer.Body, answer.IsInstructorAnswer, answer.CreatedAt);
    }


    public async Task<SectionManagementDto> CreateSectionAsync(string userId, bool isAdmin, int courseId, UpsertSectionRequest request, CancellationToken ct = default)
    {
        var course = await _db.Courses.FindAsync([courseId], ct) ?? throw new NotFoundException(nameof(Course), courseId);
        EnsureCourseOwner(course, userId, isAdmin);
        var section = new Section { CourseId = courseId, Title = request.Title, Order = request.Order };
        _db.Sections.Add(section);
        await _db.SaveChangesAsync(ct);
        return new SectionManagementDto(section.Id, section.CourseId, section.Title, section.Order);
    }

    public async Task UpdateSectionAsync(string userId, bool isAdmin, int sectionId, UpsertSectionRequest request, CancellationToken ct = default)
    {
        var section = await _db.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.Id == sectionId, ct) ?? throw new NotFoundException(nameof(Section), sectionId);
        EnsureCourseOwner(section.Course!, userId, isAdmin);
        section.Title = request.Title;
        section.Order = request.Order;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteSectionAsync(string userId, bool isAdmin, int sectionId, CancellationToken ct = default)
    {
        var section = await _db.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.Id == sectionId, ct) ?? throw new NotFoundException(nameof(Section), sectionId);
        EnsureCourseOwner(section.Course!, userId, isAdmin);
        _db.Sections.Remove(section);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<LessonManagementDto> CreateLessonAsync(string userId, bool isAdmin, int sectionId, UpsertLessonRequest request, CancellationToken ct = default)
    {
        var section = await _db.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.Id == sectionId, ct) ?? throw new NotFoundException(nameof(Section), sectionId);
        EnsureCourseOwner(section.Course!, userId, isAdmin);
        var lesson = new Lesson
        {
            SectionId = sectionId,
            Title = request.Title,
            VideoUrl = request.VideoUrl,
            DurationSeconds = request.DurationSeconds,
            Order = request.Order,
            IsFreePreview = request.IsFreePreview,
            ContentType = string.IsNullOrWhiteSpace(request.ContentType) ? "Video" : request.ContentType,
            ArticleHtml = request.ArticleHtml,
            ResourceUrl = request.ResourceUrl
        };
        _db.Lessons.Add(lesson);
        await _db.SaveChangesAsync(ct);
        return MapLesson(lesson);
    }

    public async Task UpdateLessonAsync(string userId, bool isAdmin, int lessonId, UpsertLessonRequest request, CancellationToken ct = default)
    {
        var lesson = await _db.Lessons.Include(l => l.Section).ThenInclude(s => s!.Course).FirstOrDefaultAsync(l => l.Id == lessonId, ct) ?? throw new NotFoundException(nameof(Lesson), lessonId);
        EnsureCourseOwner(lesson.Section!.Course!, userId, isAdmin);
        lesson.Title = request.Title;
        lesson.VideoUrl = request.VideoUrl;
        lesson.DurationSeconds = request.DurationSeconds;
        lesson.Order = request.Order;
        lesson.IsFreePreview = request.IsFreePreview;
        lesson.ContentType = string.IsNullOrWhiteSpace(request.ContentType) ? "Video" : request.ContentType;
        lesson.ArticleHtml = request.ArticleHtml;
        lesson.ResourceUrl = request.ResourceUrl;
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteLessonAsync(string userId, bool isAdmin, int lessonId, CancellationToken ct = default)
    {
        var lesson = await _db.Lessons.Include(l => l.Section).ThenInclude(s => s!.Course).FirstOrDefaultAsync(l => l.Id == lessonId, ct) ?? throw new NotFoundException(nameof(Lesson), lessonId);
        EnsureCourseOwner(lesson.Section!.Course!, userId, isAdmin);
        _db.Lessons.Remove(lesson);
        await _db.SaveChangesAsync(ct);
    }

    public async Task ReorderLessonsAsync(string userId, bool isAdmin, int sectionId, ReorderLessonsRequest request, CancellationToken ct = default)
    {
        var section = await _db.Sections.Include(s => s.Course).FirstOrDefaultAsync(s => s.Id == sectionId, ct) ?? throw new NotFoundException(nameof(Section), sectionId);
        EnsureCourseOwner(section.Course!, userId, isAdmin);
        var lessonIds = request.Items.Select(i => i.Id).ToHashSet();
        var lessons = await _db.Lessons.Where(l => l.SectionId == sectionId && lessonIds.Contains(l.Id)).ToListAsync(ct);
        foreach (var item in request.Items)
        {
            var lesson = lessons.FirstOrDefault(l => l.Id == item.Id);
            if (lesson is not null) lesson.Order = item.Order;
        }
        await _db.SaveChangesAsync(ct);
    }


    private static LessonManagementDto MapLesson(Lesson lesson) => new(lesson.Id, lesson.SectionId, lesson.Title, lesson.VideoUrl, lesson.DurationSeconds, lesson.Order, lesson.IsFreePreview, lesson.ContentType, lesson.ArticleHtml, lesson.ResourceUrl);

    private static void EnsureCourseOwner(Course course, string userId, bool isAdmin)
    {
        if (!isAdmin && course.TeacherId != userId)
            throw new ForbiddenException("Only the course owner or admin can modify this course structure.");
    }
    private static CouponDto MapCoupon(Coupon c) => new(c.Id, c.Code, c.DiscountType, c.DiscountValue, c.StartsAt, c.ExpiresAt, c.IsActive, c.UsedCount, c.MaxUses);
    private static LiveSessionDto MapLiveSession(LiveSession l) => new(l.Id, l.CourseId, l.Title, l.Description, l.StartTime, l.EndTime, l.MeetingUrl, l.Status, l.IsRecorded, l.RecordingUrl);
    private static OrderDto MapOrder(Order o) => new(o.Id, o.Subtotal, o.DiscountAmount, o.Total, o.CouponCode, o.Status,
        o.Items.Select(i => new OrderItemDto(i.CourseId, i.Course?.Title ?? string.Empty, i.Price)).ToList());
}
