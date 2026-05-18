using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Enums;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders;

/// <summary>
/// Seeds advanced LMS demo data used by dashboards and module views.
/// 
/// This creates test data for:
/// - coupons
/// - wishlist
/// - cart
/// - orders/order items
/// - payments
/// - lesson progress
/// - certificates
/// - live sessions
/// - notifications
/// - course questions and answers
/// </summary>
public static class LmsDemoSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        var students = (await userManager.GetUsersInRoleAsync("Student")).ToList();
        var teachers = (await userManager.GetUsersInRoleAsync("Teacher")).ToList();

        var courses = await context.Courses
            .Include(c => c.Sections)
                .ThenInclude(s => s.Lessons)
            .OrderBy(c => c.Id)
            .ToListAsync();

        if (students.Count == 0 || courses.Count == 0)
            return;

        await SeedCouponsAsync(context);
        await SeedWishlistAsync(context, students, courses);
        await SeedCartAsync(context, students, courses);
        await SeedOrdersAndPaymentsAsync(context, students, courses);
        await SeedProgressAndCertificatesAsync(context, students, courses);
        await SeedLiveSessionsAsync(context, teachers, courses);
        await SeedNotificationsAsync(context, students, teachers);
        await SeedQuestionsAsync(context, students, teachers, courses);

        await context.SaveChangesAsync();
    }

    private static async Task SeedCouponsAsync(AppDbContext context)
    {
        if (await context.Coupons.IgnoreQueryFilters().AnyAsync())
            return;

        context.Coupons.AddRange(
            new Coupon
            {
                Code = "WELCOME10",
                DiscountType = CouponDiscountType.Percentage,
                DiscountValue = 10,
                StartsAt = DateTime.UtcNow.AddDays(-7),
                ExpiresAt = DateTime.UtcNow.AddMonths(3),
                IsActive = true,
                MaxUses = 500
            },
            new Coupon
            {
                Code = "SAVE5KD",
                DiscountType = CouponDiscountType.FixedAmount,
                DiscountValue = 5,
                StartsAt = DateTime.UtcNow.AddDays(-7),
                ExpiresAt = DateTime.UtcNow.AddMonths(1),
                IsActive = true,
                MaxUses = 200
            });
    }

    private static async Task SeedWishlistAsync(
        AppDbContext context,
        List<ApplicationUser> students,
        List<Course> courses)
    {
        if (await context.WishlistItems.IgnoreQueryFilters().AnyAsync())
            return;

        foreach (var student in students.Take(5))
        {
            foreach (var course in courses.Skip(1).Take(2))
            {
                context.WishlistItems.Add(new WishlistItem
                {
                    UserId = student.Id,
                    CourseId = course.Id
                });
            }
        }
    }

    private static async Task SeedCartAsync(
        AppDbContext context,
        List<ApplicationUser> students,
        List<Course> courses)
    {
        if (await context.CartItems.IgnoreQueryFilters().AnyAsync())
            return;

        foreach (var student in students.Take(3))
        {
            foreach (var course in courses.Skip(2).Take(2))
            {
                context.CartItems.Add(new CartItem
                {
                    UserId = student.Id,
                    CourseId = course.Id,
                    PriceSnapshot = course.Price
                });
            }
        }
    }

    private static async Task SeedOrdersAndPaymentsAsync(
        AppDbContext context,
        List<ApplicationUser> students,
        List<Course> courses)
    {
        if (await context.Orders.IgnoreQueryFilters().AnyAsync())
            return;

        foreach (var student in students.Take(5))
        {
            var selectedCourses = courses.Take(2).ToList();
            var subtotal = selectedCourses.Sum(c => c.Price);
            var discount = 5m;
            var total = subtotal - discount;

            var order = new Order
            {
                UserId = student.Id,
                Subtotal = subtotal,
                DiscountAmount = discount,
                CouponCode = "SAVE5KD",
                Total = total,
                Status = OrderStatus.Paid
            };

            foreach (var course in selectedCourses)
            {
                order.Items.Add(new OrderItem
                {
                    CourseId = course.Id,
                    Price = course.Price
                });
            }

            context.Orders.Add(order);

            foreach (var course in selectedCourses)
            {
                context.Payments.Add(new Payment
                {
                    UserId = student.Id,
                    CourseId = course.Id,
                    Amount = course.Price,
                    Gateway = "DemoGateway",
                    GatewayTransactionId = $"DEMO-{student.Id[..Math.Min(6, student.Id.Length)]}-{course.Id}",
                    Status = PaymentStatus.Paid
                });
            }
        }
    }

    private static async Task SeedProgressAndCertificatesAsync(
        AppDbContext context,
        List<ApplicationUser> students,
        List<Course> courses)
    {
        if (await context.LessonProgresses.IgnoreQueryFilters().AnyAsync())
            return;

        foreach (var student in students.Take(3))
        {
            foreach (var course in courses.Take(2))
            {
                var lessons = course.Sections
                    .OrderBy(s => s.Order)
                    .SelectMany(s => s.Lessons.OrderBy(l => l.Order))
                    .ToList();

                foreach (var lesson in lessons)
                {
                    context.LessonProgresses.Add(new LessonProgress
                    {
                        StudentId = student.Id,
                        CourseId = course.Id,
                        LessonId = lesson.Id,
                        IsCompleted = true,
                        CompletedAt = DateTime.UtcNow.AddDays(-Random.Shared.Next(1, 20)),
                        WatchedSeconds = lesson.DurationSeconds
                    });
                }

                context.Certificates.Add(new Certificate
                {
                    StudentId = student.Id,
                    CourseId = course.Id,
                    CertificateNumber = $"EDU-{course.Id}-{student.Id[..Math.Min(6, student.Id.Length)].ToUpper()}",
                    IssuedAt = DateTime.UtcNow.AddDays(-1),
                    Status = CertificateStatus.Issued,
                    PdfUrl = $"/certificates/demo-{course.Id}.pdf"
                });
            }
        }
    }

    private static async Task SeedLiveSessionsAsync(
        AppDbContext context,
        List<ApplicationUser> teachers,
        List<Course> courses)
    {
        if (await context.LiveSessions.IgnoreQueryFilters().AnyAsync())
            return;

        foreach (var course in courses.Take(6))
        {
            var instructorId = !string.IsNullOrWhiteSpace(course.TeacherId)
                ? course.TeacherId
                : teachers.FirstOrDefault()?.Id ?? string.Empty;

            context.LiveSessions.Add(new LiveSession
            {
                CourseId = course.Id,
                InstructorId = instructorId,
                Title = $"Live Q&A - {course.Title}",
                Description = "Weekly live session for questions and practice.",
                StartTime = DateTime.UtcNow.AddDays(7).Date.AddHours(18),
                EndTime = DateTime.UtcNow.AddDays(7).Date.AddHours(19),
                MeetingUrl = "https://meet.example.com/eduplatform-demo",
                Status = LiveSessionStatus.Scheduled,
                MaxStudents = 50,
                IsRecorded = false
            });
        }
    }

    private static async Task SeedNotificationsAsync(
        AppDbContext context,
        List<ApplicationUser> students,
        List<ApplicationUser> teachers)
    {
        if (await context.Notifications.IgnoreQueryFilters().AnyAsync())
            return;

        foreach (var user in students.Take(5).Concat(teachers.Take(3)))
        {
            context.Notifications.Add(new Notification
            {
                UserId = user.Id,
                Title = "Welcome to EduPlatform",
                Message = "Your demo account is ready for testing dashboards and LMS workflows.",
                Url = "/Dashboard",
                IsRead = false
            });
        }
    }

    private static async Task SeedQuestionsAsync(
        AppDbContext context,
        List<ApplicationUser> students,
        List<ApplicationUser> teachers,
        List<Course> courses)
    {
        if (await context.CourseQuestions.IgnoreQueryFilters().AnyAsync())
            return;

        var student = students.First();
        var teacher = teachers.FirstOrDefault();

        foreach (var course in courses.Take(3))
        {
            var question = new CourseQuestion
            {
                CourseId = course.Id,
                StudentId = student.Id,
                Title = $"Question about {course.Title}",
                Body = "Can you explain the main concept again with another example?",
                Status = QuestionStatus.Answered
            };

            question.Answers.Add(new CourseAnswer
            {
                UserId = teacher?.Id ?? course.TeacherId,
                Body = "Sure. Review the practice lesson and attend the next live session for more examples.",
                IsInstructorAnswer = true
            });

            context.CourseQuestions.Add(question);
        }
    }
}
