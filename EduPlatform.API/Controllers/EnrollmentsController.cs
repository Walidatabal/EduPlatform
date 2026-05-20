using EduPlatform.Application.Common.Exceptions;
using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Enrollments.Commands;
using EduPlatform.Application.Features.Enrollments.DTOs;
using EduPlatform.Domain.Constants;
using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Enums;
using EduPlatform.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class EnrollmentsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public EnrollmentsController(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow; _currentUser = currentUser;
    }

    /// <summary>Enroll current student in a course (free courses only for now)</summary>
    [HttpPost]
    [Authorize(Roles = AppRoles.Student)]
    [ProducesResponseType(typeof(EnrollmentDto), 201)]
    [ProducesResponseType(400), ProducesResponseType(404)]
    public async Task<IActionResult> Enroll([FromBody] EnrollCommand cmd, CancellationToken ct)
    {
        var course = await _uow.Courses.GetByIdAsync(cmd.CourseId, ct) ?? throw new NotFoundException(nameof(Course), cmd.CourseId);

        if (course.Status != CourseStatus.Published || course.ApprovalStatus != ApprovalStatus.Approved)
            return BadRequest(new { message = "Course is not available for enrollment." });

        if (await _uow.Enrollments.IsEnrolledAsync(_currentUser.UserId!, cmd.CourseId, ct))
            return BadRequest(new { message = "Already enrolled in this course." });

        if (course.Price > 0)
            return BadRequest(new { message = "Paid courses require payment. Use the /api/payments endpoint." });

        var enrollment = new Enrollment
        {
            StudentId = _currentUser.UserId!, CourseId = cmd.CourseId,
            Status = EnrollmentStatus.Active, AmountPaid = 0, PaidAt = DateTime.UtcNow
        };
        await _uow.Enrollments.AddAsync(enrollment, ct);
        await _uow.SaveChangesAsync(ct);

        return CreatedAtAction(nameof(GetMyEnrollments), new EnrollmentDto
        {
            Id = enrollment.Id, StudentId = enrollment.StudentId, CourseId = enrollment.CourseId,
            CourseTitle = course.Title, Status = enrollment.Status, AmountPaid = enrollment.AmountPaid,
            CreatedAt = enrollment.CreatedAt
        });
    }

    /// <summary>Get all enrollments for the current user</summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<EnrollmentDto>), 200)]
    public async Task<IActionResult> GetMyEnrollments(CancellationToken ct)
    {
        var enrollments = await _uow.Enrollments.GetByStudentAsync(_currentUser.UserId!, ct);
        var dtos = enrollments.Select(e => new EnrollmentDto
        {
            Id = e.Id, StudentId = e.StudentId, CourseId = e.CourseId,
            CourseTitle = e.Course?.Title ?? string.Empty, ThumbnailUrl = e.Course?.ThumbnailUrl,
            Status = e.Status, AmountPaid = e.AmountPaid, CreatedAt = e.CreatedAt
        });
        return Ok(dtos);
    }

    /// <summary>Check if current user is enrolled in a course</summary>
    [HttpGet("check/{courseId:int}")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> CheckEnrollment(int courseId, CancellationToken ct)
    {
        var isEnrolled = await _uow.Enrollments.IsEnrolledAsync(_currentUser.UserId!, courseId, ct);
        return Ok(new { isEnrolled });
    }
}
