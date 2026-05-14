using EduPlatform.Application.Common.Exceptions;
using EduPlatform.Application.Features.Grades.Commands;
using EduPlatform.Application.Features.Grades.DTOs;
using EduPlatform.Domain.Constants;
using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class GradesController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public GradesController(IUnitOfWork uow) => _uow = uow;

    /// <summary>Get all grades</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GradeDto>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var grades = await _uow.Grades.GetAllAsync(ct);
        var dtos = grades.Select(g => new GradeDto
        {
            Id = g.Id, Name = g.Name, Description = g.Description,
            SubjectCount = g.Subjects.Count
        });
        return Ok(dtos);
    }

    /// <summary>Get a grade by ID with its subjects</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GradeDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var grade = await _uow.Grades.GetWithSubjectsAsync(id, ct)
            ?? throw new NotFoundException(nameof(Grade), id);
        return Ok(new GradeDto { Id = grade.Id, Name = grade.Name, Description = grade.Description, SubjectCount = grade.Subjects.Count });
    }

    /// <summary>Create a new grade (Admin only)</summary>
    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(typeof(GradeDto), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Create([FromBody] CreateGradeCommand cmd, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (await _uow.Grades.NameExistsAsync(cmd.Name, null, ct))
            return BadRequest(new { message = "A grade with this name already exists." });

        var grade = new Grade { Name = cmd.Name, Description = cmd.Description };
        await _uow.Grades.AddAsync(grade, ct);
        await _uow.SaveChangesAsync(ct);
        return CreatedAtAction(nameof(GetById), new { id = grade.Id },
            new GradeDto { Id = grade.Id, Name = grade.Name, Description = grade.Description });
    }

    /// <summary>Update a grade (Admin only)</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateGradeCommand cmd, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var grade = await _uow.Grades.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Grade), id);
        if (await _uow.Grades.NameExistsAsync(cmd.Name, id, ct))
            return BadRequest(new { message = "A grade with this name already exists." });

        grade.Name = cmd.Name; grade.Description = cmd.Description;
        await _uow.Grades.UpdateAsync(grade, ct);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }

    /// <summary>Soft-delete a grade (Admin only)</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = AppRoles.Admin)]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var grade = await _uow.Grades.GetByIdAsync(id, ct) ?? throw new NotFoundException(nameof(Grade), id);
        await _uow.Grades.DeleteAsync(grade, ct);
        await _uow.SaveChangesAsync(ct);
        return NoContent();
    }
}
