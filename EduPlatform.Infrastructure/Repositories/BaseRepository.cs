using System.Linq.Expressions;
using EduPlatform.Domain.Common;
using EduPlatform.Domain.Interfaces;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of the generic IRepository&lt;T&gt; contract.
///
/// Responsibilities:
/// - Provides standard CRUD operations backed by EF Core DbSet&lt;T&gt;.
/// - Used for the majority of entities where no complex query logic is needed.
/// - Extended by specialized repositories (CourseRepository, GradeRepository, etc.)
///   when modules need multi-Include queries or business-specific filters.
///
/// AsNoTracking:
/// All read methods (GetAllAsync, FindAsync, FirstOrDefaultAsync) use
/// AsNoTracking() for performance. This tells EF Core not to add the
/// returned entities to its change tracker, saving memory and CPU on read-only paths.
///
/// When you need to update an entity:
/// - Load it WITHOUT AsNoTracking (use GetByIdAsync or a tracked query).
/// - Modify properties.
/// - Call UpdateAsync (marks it Modified in the tracker).
/// - Call UnitOfWork.SaveChangesAsync().
///
/// Soft-delete:
/// DeleteAsync issues a physical DbSet.Remove(), which produces a SQL DELETE.
/// For soft-delete (preferred), set entity.IsDeleted = true and call UpdateAsync.
/// The global query filter (.HasQueryFilter(e => !e.IsDeleted)) will hide the
/// record from all subsequent queries automatically.
/// </summary>
/// <typeparam name="T">A domain entity inheriting <see cref="BaseEntity"/>.</typeparam>
public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    /// <summary>The shared AppDbContext for this request (Scoped lifetime).</summary>
    protected readonly AppDbContext _db;

    /// <summary>
    /// The typed DbSet for this entity, cached for performance.
    /// Equivalent to _db.Set&lt;T&gt;() but resolved once in the constructor.
    /// </summary>
    protected readonly DbSet<T> _set;

    public BaseRepository(AppDbContext db)
    {
        _db = db;
        _set = db.Set<T>();
    }

    /// <summary>
    /// Finds the entity by primary key. EF Core checks the change tracker first
    /// (returns the tracked instance if already loaded), then queries the database.
    /// Returns null if not found or soft-deleted (the global query filter applies).
    /// </summary>
    public async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _set.FindAsync([id], ct);

    /// <summary>
    /// Returns all non-deleted records using AsNoTracking for performance.
    /// Warning: do not call this on large tables without a WHERE clause.
    /// Use FindAsync with a predicate for filtered queries.
    /// </summary>
    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _set.AsNoTracking().ToListAsync(ct);

    /// <summary>
    /// Applies a LINQ WHERE predicate and returns matching records.
    /// Uses AsNoTracking — returned objects cannot be tracked for updates.
    /// </summary>
    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
        => await _set.AsNoTracking().Where(predicate).ToListAsync(ct);

    /// <summary>
    /// Returns the first matching record, or null if none match.
    /// More efficient than FindAsync(...).FirstOrDefault() — stops at the first hit.
    /// Does not use AsNoTracking — returns a tracked entity suitable for update.
    /// </summary>
    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
        => await _set.FirstOrDefaultAsync(predicate, ct);

    /// <summary>
    /// Adds the entity to EF Core's change tracker as "Added".
    /// The INSERT is executed when UnitOfWork.SaveChangesAsync() is called.
    /// The entity's Id is populated after SaveChangesAsync returns.
    /// </summary>
    public async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _set.AddAsync(entity, ct);
        return entity;
    }

    /// <summary>
    /// Marks all properties of the entity as "Modified" in the change tracker.
    /// EF Core will issue a full UPDATE on SaveChangesAsync.
    /// The entity must have been loaded with tracking enabled for EF Core to
    /// know which record to update.
    /// </summary>
    public Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _db.Entry(entity).State = EntityState.Modified;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Marks the entity for physical deletion (SQL DELETE) on SaveChangesAsync.
    /// Prefer soft-delete: set entity.IsDeleted = true + UpdateAsync for most cases.
    /// Only use this for truly transient records (cart items, temp sessions).
    /// </summary>
    public Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _set.Remove(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Returns true if at least one record matches the predicate.
    /// Uses SQL EXISTS under the hood — more efficient than Count > 0.
    /// </summary>
    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct = default)
        => await _set.AnyAsync(predicate, ct);

    /// <summary>
    /// Returns the COUNT of records matching the predicate.
    /// If predicate is null, counts all non-deleted records in the table
    /// (the global soft-delete query filter still applies).
    /// </summary>
    public async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken ct = default)
        => predicate == null
            ? await _set.CountAsync(ct)
            : await _set.CountAsync(predicate, ct);
}
