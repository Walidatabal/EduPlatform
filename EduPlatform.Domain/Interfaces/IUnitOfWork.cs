using EduPlatform.Domain.Entities;

namespace EduPlatform.Domain.Interfaces;

/// <summary>
/// Enterprise Unit of Work contract.
///
/// What is the Unit of Work pattern?
/// The Unit of Work (UoW) tracks all changes made to entities during a single
/// business operation and commits them as one atomic database transaction.
/// If any step fails, none of the changes are persisted — preventing partial writes
/// that would leave the database in an inconsistent state.
///
/// Why use it instead of injecting repositories directly?
/// Without UoW, a service that needs to update a Cart AND create a Notification
/// would inject two separate repositories and call SaveChangesAsync twice.
/// If the second call fails, the cart update is already committed and cannot
/// be rolled back. With UoW, both changes are queued and committed together.
///
/// Repository grouping:
/// - Specialized repositories are used for modules that need complex EF Core
///   queries with multiple .Include() calls, business filters, or projections.
/// - Generic IRepository&lt;T&gt; is used for modules that only need basic CRUD.
///
/// Registered as Scoped in DependencyInjection.cs — one instance per HTTP request.
/// Each request gets its own AppDbContext (also Scoped), so the UoW's change
/// tracker is isolated per request.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // ── Specialized repositories ────────────────────────────────────────────
    // These add custom query methods on top of the generic CRUD.

    /// <summary>Grades with subject loading and name uniqueness checks.</summary>
    IGradeRepository Grades { get; }

    /// <summary>Subjects filtered by grade, with grade name uniqueness checks.</summary>
    ISubjectRepository Subjects { get; }

    /// <summary>
    /// Courses with multi-Include queries (GetWithDetailsAsync loads
    /// Subject → Grade, Category, Reviews, Sections → Lessons) and
    /// status-filtered published/pending queries.
    /// </summary>
    ICourseRepository Courses { get; }

    /// <summary>Enrollments with enrollment-status filtering and student lookups.</summary>
    IEnrollmentRepository Enrollments { get; }

    // ── Generic repositories ────────────────────────────────────────────────
    // These provide standard CRUD via BaseRepository<T>.

    /// <summary>Course categories (Programming, Design, Languages, etc.).</summary>
    IRepository<Category> Categories { get; }

    /// <summary>
    /// Shopping cart items.
    /// Each record stores a PriceSnapshot to freeze the price at add-to-cart time.
    /// </summary>
    IRepository<CartItem> CartItems { get; }

    /// <summary>Student wishlist items (saved courses for later purchase).</summary>
    IRepository<WishlistItem> WishlistItems { get; }

    /// <summary>Discount coupons with code, discount type, and expiry.</summary>
    IRepository<Coupon> Coupons { get; }

    /// <summary>Student course reviews and star ratings.</summary>
    IRepository<CourseReview> CourseReviews { get; }

    /// <summary>
    /// Purchase orders.
    /// Each order stores a financial snapshot (Subtotal, DiscountAmount, Total)
    /// so the receipt is accurate regardless of future price changes.
    /// </summary>
    IRepository<Order> Orders { get; }

    /// <summary>
    /// Individual line items within an order.
    /// Stores CourseTitle and Price at purchase time for immutable receipts.
    /// </summary>
    IRepository<OrderItem> OrderItems { get; }

    /// <summary>Payment records linked to orders (for payment gateway integration).</summary>
    IRepository<Payment> Payments { get; }

    /// <summary>In-app notifications sent to users.</summary>
    IRepository<Notification> Notifications { get; }

    /// <summary>Per-student, per-lesson progress tracking records.</summary>
    IRepository<LessonProgress> LessonProgress { get; }

    /// <summary>Completion certificates issued to students.</summary>
    IRepository<Certificate> Certificates { get; }

    /// <summary>Live sessions (Zoom/Meet) scheduled for courses.</summary>
    IRepository<LiveSession> LiveSessions { get; }

    /// <summary>Course sections (modules / chapters).</summary>
    IRepository<Section> Sections { get; }

    /// <summary>Individual lessons within sections (video, PDF, text, quiz).</summary>
    IRepository<Lesson> Lessons { get; }

    /// <summary>Student Q&amp;A questions on lessons.</summary>
    IRepository<CourseQuestion> CourseQuestions { get; }

    /// <summary>Teacher/instructor answers to student questions.</summary>
    IRepository<CourseAnswer> CourseAnswers { get; }

    /// <summary>
    /// Commits all pending changes from all repositories in one database transaction.
    ///
    /// Call this once per business operation, after all AddAsync / UpdateAsync /
    /// DeleteAsync calls are complete. If an exception is thrown, no changes
    /// are persisted.
    ///
    /// Returns the number of state entries written to the database.
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
