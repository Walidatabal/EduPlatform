using EduPlatform.API.Extensions;
using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/courses/{courseId:int}/questions")]
[Authorize]
[Produces("application/json")]
public class QuestionsController : ControllerBase
{
    private readonly ILmsPlatformService _service;
    private readonly ICurrentUserService _currentUser;
    public QuestionsController(ILmsPlatformService service, ICurrentUserService currentUser)
    {
        _service = service; _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourseQuestions(int courseId, CancellationToken ct) =>
        this.ApiOk(await _service.GetCourseQuestionsAsync(courseId, ct));

    [HttpPost]
    [Authorize(Roles = AppRoles.Student)]
    public async Task<IActionResult> Ask(int courseId, [FromBody] AskQuestionRequest request, CancellationToken ct) =>
        this.ApiOk(await _service.AskQuestionAsync(_currentUser.UserId!, courseId, request, ct));

    [HttpPost("{questionId:int}/answers")]
    [Authorize(Roles = $"{AppRoles.Student},{AppRoles.Teacher},{AppRoles.Admin}")]
    public async Task<IActionResult> Answer(int courseId, int questionId, [FromBody] AnswerQuestionRequest request, CancellationToken ct) =>
        this.ApiOk(await _service.AnswerQuestionAsync(_currentUser.UserId!, _currentUser.IsInRole(AppRoles.Admin), courseId, questionId, request, ct));
}
