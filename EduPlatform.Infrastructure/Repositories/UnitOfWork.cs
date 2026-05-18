using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Interfaces;
using EduPlatform.Infrastructure.Data;

namespace EduPlatform.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the IUnitOfWork contract.
///
/// How it works:
/// All repositories in this class share the same AppDbContext instance (_db).
/// This means all their AddAsync / UpdateAsync / DeleteAsync calls queue changes
/// in the same EF Core change tracker. When SaveChangesAsync is called once,
/// all queued changes are written to the database in a single SQL transaction.
///
/// Why wrap EF Core's DbContext?
/// EF Core's DbContext is itself a Unit of Work internally, but exposing it
/// directly to services couples services to EF Core types (IQueryable, DbSet).
/// IUnitOfWork gives services an abstraction they can depend on — making them
/// testable with Mock&lt;IUnitOfWork&gt; without needing a real database.
///
/// Lifecycle:
/// Registered as Scoped in DependencyInjection.cs.
/// One UnitOfWork per HTTP request. The underlying AppDbContext is also Scoped,
/// so each request gets its own change tracker — no cross-request contamination.
///
/// Repository initialization:
/// - Specialized repositories (GradeRepository, CourseRepository etc.) add
///   domain-specific query methods on top of the generic CRUD.
/// - Generic BaseRepository&lt;T&gt; instances handle standard CRUD for entities
///   that do not need specialized queries.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    // ── Specialized repositories ────────────────────────────────────────────
    // These extend BaseRepository with domain-specific Include chains and filters.

    /// <inheritdoc/>
    public IGradeRepository Grades { get; }

    /// <inheritdoc/>
    public ISubjectRepository Subjects { get; }

    /// <inheritdoc/>
    public ICourseRepository Courses { get; }

    /// <inheritdoc/>
    public IEnrollmentRepository Enrollments { get; }

    // ── Generic repositories ────────────────────────────────────────────────
    // Standard CRUD — no domain-specific queries needed.

    /// <inheritdoc/>
    public IRepository<Category> Categories { get; }

    /// <inheritdoc/>
    public IRepository<CartItem> CartItems { get; }

    /// <inheritdoc/>
    public IRepository<WishlistItem> WishlistItems { get; }

    /// <inheritdoc/>
    public IRepository<Coupon> Coupons { get; }

    /// <inheritdoc/>
    public IRepository<CourseReview> CourseReviews { get; }

    /// <inheritdoc/>
    public IRepository<Order> Orders { get; }

    /// <inheritdoc/>
    public IRepository<OrderItem> OrderItems { get; }

    /// <inheritdoc/>
    public IRepository<Payment> Payments { get; }

    /// <inheritdoc/>
    public IRepository<Notification> Notifications { get; }

    /// <inheritdoc/>
    public IRepository<LessonProgress> LessonProgress { get; }

    /// <inheritdoc/>
    public IRepository<Certificate> Certificates { get; }

    /// <inheritdoc/>
    public IRepository<LiveSession> LiveSessions { get; }

    /// <inheritdoc/>
    public IRepository<Section> Sections { get; }

    /// <inheritdoc/>
    public IRepository<Lesson> Lessons { get; }

    /// <inheritdoc/>
    public IRepository<CourseQuestion> CourseQuestions { get; }

    /// <inheritdoc/>
    public IRepository<CourseAnswer> CourseAnswers { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;

        // ── Specialized repository initialization ───────────────────────────
        // Each specialized repository receives the same _db instance so all
        // changes share one change tracker and one transaction.
        Grades      = new GradeRepository(db);
        Subjects    = new SubjectRepository(db);
        Courses     = new CourseRepository(db);
        Enrollments = new EnrollmentRepository(db);

        // ── Generic repository initialization ───────────────────────────────
        // BaseRepository<T> handles standard CRUD for all remaining entities.
        Categories    = new BaseRepository<Category>(db);
        CartItems     = new BaseRepository<CartItem>(db);
        WishlistItems = new BaseRepository<WishlistItem>(db);
        Coupons       = new BaseRepository<Coupon>(db);
        CourseReviews = new BaseRepository<CourseReview>(db);
        Orders        = new BaseRepository<Order>(db);
        OrderItems    = new BaseRepository<OrderItem>(db);
        Payments      = new BaseRepository<Payment>(db);
        Notifications = new BaseRepository<Notification>(db);
        LessonProgress = new BaseRepository<LessonProgress>(db);
        Certificates  = new BaseRepository<Certificate>(db);
        LiveSessions  = new BaseRepository<LiveSession>(db);
        Sections      = new BaseRepository<Section>(db);
        Lessons       = new BaseRepository<Lesson>(db);
        CourseQuestions = new BaseRepository<CourseQuestion>(db);
        CourseAnswers   = new BaseRepository<CourseAnswer>(db);
    }

    /// <summary>
    /// Commits all pending changes from every repository in one SQL transaction.
    /// Returns the number of state entries written to the database.
    /// If an exception is thrown, no changes are committed (EF Core rolls back).
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);

    /// <summary>
    /// Disposes the underlying AppDbContext.
    /// Called automatically at the end of each HTTP request (Scoped lifetime).
    /// </summary>
    public void Dispose() => _db.Dispose();
}
