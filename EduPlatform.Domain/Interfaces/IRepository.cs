using System.Linq.Expressions;
using EduPlatform.Domain.Common;

namespace EduPlatform.Domain.Interfaces;

/// <summary>
/// Generic repository contract for common CRUD and query operations.
///
/// Why a repository interface on top of EF Core's DbSet?
/// - Services depend on this interface (Domain), not on EF Core types (Infrastructure).
///   This means service tests can use Mock&lt;IRepository&lt;T&gt;&gt; without a real database.
/// - Complex queries (multi-table Include chains, business-specific filters) are
///   encapsulated in specialized repositories, keeping services clean.
/// - If the underlying ORM ever changes (e.g. from EF Core to Dapper), only
///   BaseRepository changes — services are unaffected.
///
/// Constraint: T must inherit BaseEntity to guarantee every entity has Id,
/// CreatedAt, UpdatedAt, and IsDeleted.
///
/// Usage rule:
/// - Use the generic IRepository&lt;T&gt; for simple CRUD with no complex includes.
/// - Use specialized repositories (ICourseRepository, IGradeRepository, etc.)
///   when the module needs multi-level Includes, filtering by status, or
///   business-specific projection queries.
/// </summary>
/// <typeparam name="T">A domain entity type inheriting <see cref="BaseEntity"/>.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Returns the entity with the given primary key, or null if not found.
    /// Uses EF Core's FindAsync — checks the change tracker first, then the database.
    /// </summary>
    Task<T?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Returns all non-deleted records. Uses AsNoTracking for read-only performance.
    /// Warning: do not call on large tables without a filter — use FindAsync instead.
    /// </summary>
    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Returns all records matching the given predicate. Uses AsNoTracking.
    /// Example: await uow.Courses.FindAsync(c => c.TeacherId == userId)
    /// </summary>
    Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default);

    /// <summary>
    /// Returns the first record matching the predicate, or null.
    /// More efficient than FindAsync when only one result is needed.
    /// </summary>
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default);

    /// <summary>
    /// Adds the entity to the EF Core change tracker.
    /// The record is not persisted to the database until SaveChangesAsync is called.
    /// Returns the entity so the caller can read the assigned Id after save.
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Marks the entity as Modified in the EF Core change tracker.
    /// The record is not updated in the database until SaveChangesAsync is called.
    /// Entity must be tracked (loaded with tracking) for this to work correctly.
    /// </summary>
    Task UpdateAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Marks the entity for removal from the EF Core change tracker.
    /// Issues a physical DELETE — prefer soft-delete via IsDeleted = true for most cases.
    /// The record is not removed from the database until SaveChangesAsync is called.
    /// </summary>
    Task DeleteAsync(T entity, CancellationToken ct = default);

    /// <summary>
    /// Returns true if any record matches the predicate.
    /// More efficient than FindAsync(...).Any() as it stops at the first match.
    /// </summary>
    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default);

    /// <summary>
    /// Returns the count of records matching the optional predicate.
    /// If predicate is null, counts all non-deleted records in the table.
    /// </summary>
    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default);
}
