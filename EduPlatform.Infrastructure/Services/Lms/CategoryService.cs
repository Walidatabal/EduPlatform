using EduPlatform.Application.Common.Exceptions;
using EduPlatform.Application.Common.Results;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Entities;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Services.Lms;

/// <summary>
/// Infrastructure implementation of category business operations.
/// 
/// WHY INSIDE INFRASTRUCTURE?
/// Because this class directly depends on:
/// - EF Core
/// - DbContext
/// - Database access
/// 
/// Application layer only defines the contract (ICategoryService).
/// Infrastructure provides the actual implementation.
/// </summary>
public class CategoryService : ICategoryService
{
    /// <summary>
    /// EF Core database context.
    /// Used for querying and saving data.
    /// </summary>
    private readonly AppDbContext _db;

    /// <summary>
    /// Constructor Injection.
    /// 
    /// Dependency Injection automatically provides AppDbContext.
    /// </summary>
    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns all categories from database.
    /// 
    /// AsNoTracking():
    /// Improves performance because entities are read-only.
    /// 
    /// Select():
    /// Converts Entity -> DTO
    /// to avoid exposing database entities directly.
    /// </summary>
    public async Task<ServiceResult<IReadOnlyList<CategoryDto>>> GetCategoriesAsync(
        CancellationToken ct = default)
    {
        var categories = await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.IconUrl,
                c.ParentCategoryId))
            .ToListAsync(ct);

        return ServiceResult<IReadOnlyList<CategoryDto>>
            .Ok(categories, "Categories loaded successfully.");
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    public async Task<ServiceResult<CategoryDto>> CreateCategoryAsync(
        UpsertCategoryRequest request,
        CancellationToken ct = default)
    {
        /// Create domain entity
        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            IconUrl = request.IconUrl,
            ParentCategoryId = request.ParentCategoryId
        };

        /// Add entity to EF tracking
        _db.Categories.Add(category);

        /// Save changes to database
        await _db.SaveChangesAsync(ct);

        /// Convert Entity -> DTO
        var dto = new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.IconUrl,
            category.ParentCategoryId);

        /// Return standardized success response
        return ServiceResult<CategoryDto>
            .Ok(dto, "Category created successfully.");
    }

    /// <summary>
    /// Updates existing category.
    /// </summary>
    public async Task<ServiceResult<bool>> UpdateCategoryAsync(
        int id,
        UpsertCategoryRequest request,
        CancellationToken ct = default)
    {
        /// Find entity by primary key
        var category = await _db.Categories.FindAsync([id], ct)

            /// Throw enterprise custom exception if not found
            ?? throw new NotFoundException(nameof(Category), id);

        /// Update entity values
        category.Name = request.Name.Trim();
        category.Description = request.Description;
        category.IconUrl = request.IconUrl;
        category.ParentCategoryId = request.ParentCategoryId;

        /// Save updated values
        await _db.SaveChangesAsync(ct);

        return ServiceResult<bool>
            .Ok(true, "Category updated successfully.");
    }

    /// <summary>
    /// Deletes category from database.
    /// </summary>
    public async Task<ServiceResult<bool>> DeleteCategoryAsync(
        int id,
        CancellationToken ct = default)
    {
        var category = await _db.Categories.FindAsync([id], ct)

            ?? throw new NotFoundException(nameof(Category), id);

        /// Remove entity
        _db.Categories.Remove(category);

        /// Commit deletion
        await _db.SaveChangesAsync(ct);

        return ServiceResult<bool>
            .Ok(true, "Category deleted successfully.");
    }
}