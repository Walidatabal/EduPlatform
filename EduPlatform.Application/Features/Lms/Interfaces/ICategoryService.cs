using EduPlatform.Application.Common.Results;
using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

/// <summary>
/// Application contract for Category operations.
/// 
/// IMPORTANT:
/// Interface belongs to Application Layer,
/// because Application defines the business contracts,
/// while Infrastructure implements them.
/// 
/// CLEAN ARCHITECTURE RULE:
/// High-level layers define abstractions.
/// Low-level layers implement them.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Returns all categories.
    /// </summary>
    Task<ServiceResult<IReadOnlyList<CategoryDto>>> GetCategoriesAsync(
        CancellationToken ct = default);

    /// <summary>
    /// Creates a new category.
    /// </summary>
    Task<ServiceResult<CategoryDto>> CreateCategoryAsync(
        UpsertCategoryRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Updates an existing category.
    /// </summary>
    Task<ServiceResult<bool>> UpdateCategoryAsync(
        int id,
        UpsertCategoryRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Deletes a category.
    /// </summary>
    Task<ServiceResult<bool>> DeleteCategoryAsync(
        int id,
        CancellationToken ct = default);
}