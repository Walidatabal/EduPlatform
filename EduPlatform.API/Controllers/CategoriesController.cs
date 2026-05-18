using EduPlatform.API.Extensions;
using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

/// <summary>
/// API controller responsible for Category endpoints.
/// 
/// Controller responsibility:
/// - Receive HTTP request
/// - Call Application service
/// - Return HTTP response
/// 
/// Controller should NOT contain business logic.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CategoriesController : ControllerBase
{
    /// <summary>
    /// Main LMS application service.
    /// 
    /// The controller depends on an abstraction,
    /// not directly on DbContext or Infrastructure.
    /// </summary>
    //private readonly ILmsPlatformService _service;

    /// <summary>
    /// Constructor Injection.
    /// Dependency Injection provides ILmsPlatformService implementation.
    /// </summary>
    private readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }
    /// <summary>
    /// GET: api/categories
    /// 
    /// Public endpoint to return all categories.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _categoryService.GetCategoriesAsync(ct);

        return this.ApiOk(result.Data, result.Message);
    }

    /// <summary>
    /// POST: api/categories
    /// 
    /// Only Admin or ContentManager can create categories.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ContentManager}")]
    public async Task<IActionResult> Create(
        [FromBody] UpsertCategoryRequest request,
        CancellationToken ct)
    {
        var result = await _categoryService.CreateCategoryAsync(request, ct);

        return this.ApiCreated(
            nameof(GetAll),
            new { id = result.Data!.Id },
            result.Data,
            result.Message);
    }

    /// <summary>
    /// PUT: api/categories/{id}
    /// 
    /// Updates an existing category.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.ContentManager}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] UpsertCategoryRequest request,
        CancellationToken ct)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, request, ct);

        return this.ApiOk(result.Data, result.Message);
    }

    /// <summary>
    /// DELETE: api/categories/{id}
    /// 
    /// Only Admin can delete categories.
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var result = await _categoryService.DeleteCategoryAsync(id, ct);

        return this.ApiOk(result.Data, result.Message);
    }
}