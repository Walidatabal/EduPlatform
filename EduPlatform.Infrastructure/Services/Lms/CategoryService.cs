using EduPlatform.Application.Common.Exceptions;
using EduPlatform.Application.Common.Results;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

/// <summary>
/// Infrastructure implementation of category business operations.
///
/// This service now uses UnitOfWork instead of direct AppDbContext access.
/// That keeps the service cleaner and centralizes database commits.
/// </summary>
public class CategoryService : ICategoryService
{
    private readonly IUnitOfWork _uow;

    /// <summary>
    /// Constructor Injection.
    /// Dependency Injection provides the UnitOfWork implementation.
    /// </summary>
    public CategoryService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    /// <summary>
    /// Returns all categories from database.
    /// </summary>
    public async Task<ServiceResult<IReadOnlyList<CategoryDto>>> GetCategoriesAsync(
        CancellationToken ct = default)
    {
        // Repository returns entities.
        var categories = await _uow.Categories.GetAllAsync(ct);

        // Map Entity -> DTO.
        var result = categories
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(
                c.Id,
                c.Name,
                c.Description,
                c.IconUrl,
                c.ParentCategoryId))
            .ToList();

        return ServiceResult<IReadOnlyList<CategoryDto>>
            .Ok(result, "Categories loaded successfully.");
    }

    /// <summary>
    /// Creates a new category.
    /// </summary>
    public async Task<ServiceResult<CategoryDto>> CreateCategoryAsync(
        UpsertCategoryRequest request,
        CancellationToken ct = default)
    {
        // Create domain entity from request model.
        var category = new Category
        {
            Name = request.Name.Trim(),
            Description = request.Description,
            IconUrl = request.IconUrl,
            ParentCategoryId = request.ParentCategoryId
        };

        // Add through repository, then commit through UnitOfWork.
        await _uow.Categories.AddAsync(category, ct);
        await _uow.SaveChangesAsync(ct);

        var dto = new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.IconUrl,
            category.ParentCategoryId);

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
        // Load tracked entity by id through repository.
        var category = await _uow.Categories.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Category), id);

        category.Name = request.Name.Trim();
        category.Description = request.Description;
        category.IconUrl = request.IconUrl;
        category.ParentCategoryId = request.ParentCategoryId;

        await _uow.Categories.UpdateAsync(category, ct);
        await _uow.SaveChangesAsync(ct);

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
        var category = await _uow.Categories.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Category), id);

        await _uow.Categories.DeleteAsync(category, ct);
        await _uow.SaveChangesAsync(ct);

        return ServiceResult<bool>
            .Ok(true, "Category deleted successfully.");
    }
}
