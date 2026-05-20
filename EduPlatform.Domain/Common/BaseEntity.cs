namespace EduPlatform.Domain.Common;

/// <summary>
/// Abstract base class inherited by every domain entity in EduPlatform.
///
/// Why every entity inherits this:
/// - <see cref="Id"/> gives every table a consistent integer primary key.
/// - <see cref="CreatedAt"/> and <see cref="UpdatedAt"/> enable audit trails and
///   sorted queries (e.g. "most recently created courses") without additional columns.
/// - <see cref="CreatedBy"/> / <see cref="UpdatedBy"/> store the Identity user ID
///   of whoever last touched the record, for accountability.
/// - <see cref="IsDeleted"/> enables soft-delete: records are never physically
///   removed from the database; they are flagged and hidden from queries.
///   This preserves referential integrity, supports undo/restore, and meets
///   regulatory requirements where data cannot be destroyed.
///
/// Enterprise rule:
/// AppDbContext.SaveChangesAsync intercepts all saves and automatically:
///   - Sets CreatedAt = DateTime.UtcNow on Add
///   - Sets UpdatedAt = DateTime.UtcNow on Modify
/// Never set these properties manually in services.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Auto-incremented integer primary key.
    /// EF Core sets this after INSERT.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// UTC timestamp set automatically on record creation.
    /// Always stored as UTC; convert to local time in the presentation layer.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// UTC timestamp updated automatically every time the record is modified.
    /// Null until the first update.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Identity user ID (GUID string) of the user who created this record.
    /// Set by AppDbContext from ICurrentUserService.UserId on Add.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Identity user ID (GUID string) of the user who last modified this record.
    /// Set by AppDbContext from ICurrentUserService.UserId on Modify.
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Soft-delete flag.
    /// When true, the record is excluded from all normal queries via a global
    /// EF Core query filter: .HasQueryFilter(e => !e.IsDeleted)
    ///
    /// To delete a record, set IsDeleted = true and call SaveChangesAsync.
    /// Never call DbSet.Remove() directly — that would issue a physical DELETE.
    /// </summary>
    public bool IsDeleted { get; set; }
}
