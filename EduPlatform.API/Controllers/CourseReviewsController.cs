using EduPlatform.API.Extensions;
using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/reviews")]
[Produces("application/json")]
public class CourseReviewsController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;
    public CourseReviewsController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> Get(int courseId, CancellationToken ct) =>
        this.ApiOk(await _service.GetCourseReviewsAsync(courseId, ct));

    [HttpPost]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> AddOrUpdate(int courseId, [FromBody] UpsertCourseReviewRequest request, CancellationToken ct) =>
        this.ApiOk(await _service.AddOrUpdateReviewAsync(_currentUser.UserId!, courseId, request, ct));

    [HttpDelete("{reviewId:int}")]
    [Authorize(Roles = $"{AppRoles.Student},{AppRoles.Admin}")]
    public async Task<IActionResult> Delete(int reviewId, CancellationToken ct)
    {
        await _service.DeleteReviewAsync(reviewId, _currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), ct);
        return NoContent();
    }
}
