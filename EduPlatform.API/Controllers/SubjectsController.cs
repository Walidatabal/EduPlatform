using EduPlatform.Application.Common.Exceptions;
using EduPlatform.Application.Features.Subjects.Commands;
using EduPlatform.Application.Features.Subjects.DTOs;
using EduPlatform.Domain.Constants;
using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SubjectsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    public SubjectsController(IUnitOfWork uow) => _uow = uow;

    /// <summary>Get all subjects, optionally filtered by grade</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<SubjectDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int? gradeId, CancellationToken ct)
    {
        var subjects = gradeId.HasValue
            ? await _uow.Subjects.GetByGradeAsync(gradeId.Value, ct)
            : await _uow.Subjects.GetAllAsync(ct);

        var dtos = subjects.Select(s => new SubjectDto
        {
            Id = s.Id, Name = s.Name, Description = s.Description,
            GradeId = s.GradeId, GradeName = s.Grade?.Name ?? string.Empty
        });
        return Ok(dtos);
    }

    /// <summary>Get subject by ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SubjectDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var subject = await _uow.Subjects.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Subject), id);
        return Ok(new SubjectDto { Id = subject.Id, Name = subject.Name, Description = subject.Description, GradeId = subject.GradeId });
    }

    /// <summary>Create subject (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(typeof(SubjectDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateSubjectCommand cmd, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var grade = await _uow.Grades.GetByIdAsync(cmd.GradeId, ct) ?? throw new NotFoundException(nameof(Grade), cmd.GradeId);

        var subject = new Subject { Name = cmd.Name, Description = cmd.Description, GradeId = cmd.GradeId };
        await _uow.Subjects.AddAsync(subject, ct);
        await _uow.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = subject.Id },
            new SubjectDto { Id = subject.Id, Name = subject.Name, Description = subject.Description, GradeId = subject.GradeId, GradeName = grade.Name });
    }

    /// <summary>Update subject (Admin only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectCommand cmd, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var subject = await _uow.Subjects.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Subject), id);
        if (!await _uow.Grades.AnyAsync(g => g.Id == cmd.GradeId, ct))
            throw new NotFoundException(nameof(Grade), cmd.GradeId);

        subject.Name = cmd.Name; subject.Description = cmd.Description; subject.GradeId = cmd.GradeId;
        await _uow.Subjects.UpdateAsync(subject, ct);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>Delete subject (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var subject = await _uow.Subjects.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Subject), id);
        await _uow.Subjects.DeleteAsync(subject, ct);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }
}
