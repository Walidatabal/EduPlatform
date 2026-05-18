using EduPlatform.API.Extensions;
using EduPlatform.Application.Common.Exceptions;
using EduPlatform.Application.Common.Extensions;
using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Courses.Commands;
using EduPlatform.Application.Features.Courses.DTOs;
using EduPlatform.Application.Features.Courses.Queries;
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
public class CoursesController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;
    private readonly IWebHostEnvironment _environment;

    public CoursesController(IUnitOfWork uow, ICurrentUserService currentUser, IWebHostEnvironment environment)
    {
        _uow = uow;
        _currentUser = currentUser;
        _environment = environment;
    }

    /// <summary>Browse published + approved courses (with optional filters)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CourseListDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] CourseFilterQuery query, CancellationToken ct)
    {
        var courses = await _uow.Courses.GetPublishedAsync(ct);

        if (query.GradeId.HasValue)
            courses = courses.Where(c => c.Subject?.GradeId == query.GradeId.Value).ToList();
        if (query.SubjectId.HasValue)
            courses = courses.Where(c => c.SubjectId == query.SubjectId.Value).ToList();
        if (query.CategoryId.HasValue)
            courses = courses.Where(c => c.CategoryId == query.CategoryId.Value).ToList();
        if (!string.IsNullOrWhiteSpace(query.Level))
            courses = courses.Where(c => c.Level.Equals(query.Level, StringComparison.OrdinalIgnoreCase)).ToList();
        if (query.IsFree.HasValue)
            courses = courses.Where(c => query.IsFree.Value ? c.Price == 0 : c.Price > 0).ToList();
        if (!string.IsNullOrWhiteSpace(query.Search))
            courses = courses.Where(c => c.Title.Contains(query.Search, StringComparison.OrdinalIgnoreCase)).ToList();

        var dtos = courses.Select(c => new CourseListDto
        {
            Id = c.Id, Title = c.Title, ThumbnailUrl = c.ThumbnailUrl, Price = c.Price,
            Level = c.Level, Language = c.Language, CategoryId = c.CategoryId, CategoryName = c.Category?.Name,
            AverageRating = c.Reviews.Count == 0 ? 0 : Math.Round((decimal)c.Reviews.Average(r => r.Rating), 2),
            ReviewCount = c.Reviews.Count, Status = c.Status, SubjectName = c.Subject?.Name ?? string.Empty,
            GradeName = c.Subject?.Grade?.Name ?? string.Empty, EnrollmentCount = c.Enrollments.Count
        });

        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            dtos = query.SortBy.ToLowerInvariant() switch
            {
                "price" => query.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? dtos.OrderByDescending(c => c.Price) : dtos.OrderBy(c => c.Price),
                "rating" => query.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? dtos.OrderByDescending(c => c.AverageRating) : dtos.OrderBy(c => c.AverageRating),
                _ => query.SortDirection.Equals("desc", StringComparison.OrdinalIgnoreCase) ? dtos.OrderByDescending(c => c.Title) : dtos.OrderBy(c => c.Title)
            };
        }

        return this.ApiOk(dtos.ToPagedResult(query.PageNumber, query.PageSize));
    }

    /// <summary>Get course details (full syllabus)</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CourseDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var course = await _uow.Courses.GetWithDetailsAsync(id, ct) ?? throw new NotFoundException(nameof(Course), id);
        return this.ApiOk(MapToDto(course));
    }

    /// <summary>Create a course (Teacher only)</summary>
    [HttpPost]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin},{AppRoles.ContentManager}")]
    [ProducesResponseType(typeof(CourseDto), 201)]
    [ProducesResponseType(400), ProducesResponseType(401)]
    public async Task<IActionResult> Create([FromBody] CreateCourseCommand cmd, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (!await _uow.Subjects.AnyAsync(s => s.Id == cmd.SubjectId, ct))
            throw new NotFoundException(nameof(Subject), cmd.SubjectId);

        var course = new Course
        {
            Title = cmd.Title, Description = cmd.Description, ThumbnailUrl = cmd.ThumbnailUrl,
            Price = cmd.Price, SubjectId = cmd.SubjectId, CategoryId = cmd.CategoryId,
            Level = cmd.Level, Language = cmd.Language, Requirements = cmd.Requirements, LearningOutcomes = cmd.LearningOutcomes,
            TeacherId = _currentUser.UserId!,
            Status = CourseStatus.Draft, ApprovalStatus = ApprovalStatus.Pending
        };
        await _uow.Courses.AddAsync(course, ct);
        await _uow.SaveChangesAsync(ct);
        return this.ApiCreated(nameof(GetById), new { id = course.Id }, MapToDto(course));
    }

    /// <summary>Update a course (Owner Teacher or Admin)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    [ProducesResponseType(204), ProducesResponseType(403), ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseCommand cmd, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var course = await _uow.Courses.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Course), id);

        if (course.TeacherId != _currentUser.UserId && !_currentUser.IsInRole(AppRoles.Admin))
            throw new ForbiddenException();

        course.Title = cmd.Title; course.Description = cmd.Description; course.ThumbnailUrl = cmd.ThumbnailUrl;
        course.Price = cmd.Price; course.SubjectId = cmd.SubjectId; course.CategoryId = cmd.CategoryId;
        course.Level = cmd.Level; course.Language = cmd.Language;
        course.Requirements = cmd.Requirements; course.LearningOutcomes = cmd.LearningOutcomes;
        await _uow.Courses.UpdateAsync(course, ct);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }


    /// <summary>Upload or replace course thumbnail (Owner Teacher or Admin)</summary>
    [HttpPost("{id:int}/thumbnail")]
    [Authorize(Roles = $"{AppRoles.Teacher},{AppRoles.Admin}")]
    [ProducesResponseType(typeof(object), 200), ProducesResponseType(403), ProducesResponseType(404)]
    public async Task<IActionResult> UploadThumbnail(int id, IFormFile file, CancellationToken ct)
    {
        var course = await _uow.Courses.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Course), id);
        if (course.TeacherId != _currentUser.UserId && !_currentUser.IsInRole(AppRoles.Admin))
            throw new ForbiddenException();

        if (file.Length == 0)
            return BadRequest(new { message = "Thumbnail file is empty." });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest(new { message = "Only jpg, jpeg, png, and webp thumbnails are allowed." });

        var uploadsRoot = Path.Combine(_environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"), "uploads", "course-thumbnails");
        Directory.CreateDirectory(uploadsRoot);

        var fileName = $"course-{id}-{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadsRoot, fileName);
        await using (var stream = System.IO.File.Create(fullPath))
        {
            await file.CopyToAsync(stream, ct);
        }

        course.ThumbnailUrl = $"/uploads/course-thumbnails/{fileName}";
        await _uow.Courses.UpdateAsync(course, ct);
        await _uow.SaveChangesAsync(ct);

        return this.ApiOk(new { course.Id, course.ThumbnailUrl }, "Thumbnail uploaded successfully.");
    }

    /// <summary>Submit course for admin approval (Owner Teacher)</summary>
    [HttpPost("{id:int}/submit")]
    [Authorize(Roles = AppRoles.Teacher)]
    [ProducesResponseType(204), ProducesResponseType(403), ProducesResponseType(404)]
    public async Task<IActionResult> Submit(int id, CancellationToken ct)
    {
        var course = await _uow.Courses.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Course), id);
        if (course.TeacherId != _currentUser.UserId) throw new ForbiddenException();
        course.ApprovalStatus = ApprovalStatus.Pending;
        course.Status = CourseStatus.Draft;
        await _uow.Courses.UpdateAsync(course, ct);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>Approve or reject a course (Admin only)</summary>
    [HttpPost("{id:int}/review")]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(204), ProducesResponseType(404)]
    public async Task<IActionResult> Review(int id, [FromBody] ReviewCourseRequest req, CancellationToken ct)
    {
        var course = await _uow.Courses.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Course), id);
        course.ApprovalStatus = req.Approve ? ApprovalStatus.Approved : ApprovalStatus.Rejected;
        if (req.Approve) course.Status = CourseStatus.Published;
        await _uow.Courses.UpdateAsync(course, ct);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>Delete a course (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(204), ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var course = await _uow.Courses.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Course), id);
        await _uow.Courses.DeleteAsync(course, ct);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    private static CourseDto MapToDto(Course c) => new()
    {
        Id = c.Id, Title = c.Title, Description = c.Description, ThumbnailUrl = c.ThumbnailUrl,
        Price = c.Price, Level = c.Level, Language = c.Language, Requirements = c.Requirements, LearningOutcomes = c.LearningOutcomes,
        CategoryId = c.CategoryId, CategoryName = c.Category?.Name, AverageRating = c.Reviews.Count == 0 ? 0 : Math.Round((decimal)c.Reviews.Average(r => r.Rating), 2),
        ReviewCount = c.Reviews.Count, Status = c.Status, ApprovalStatus = c.ApprovalStatus, TeacherId = c.TeacherId,
        SubjectId = c.SubjectId, SubjectName = c.Subject?.Name ?? string.Empty,
        GradeName = c.Subject?.Grade?.Name ?? string.Empty,
        EnrollmentCount = c.Enrollments.Count, SectionCount = c.Sections.Count, CreatedAt = c.CreatedAt
    };
}

public record ReviewCourseRequest(bool Approve, string? Reason = null);
