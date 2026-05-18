using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Enums;
using EduPlatform.Domain.Interfaces;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Repositories;

public class CourseRepository : BaseRepository<Course>, ICourseRepository
{
    public CourseRepository(AppDbContext db) : base(db) { }

    public async Task<Course?> GetWithDetailsAsync(int id, CancellationToken ct = default)
        => await _set
            .Include(c => c.Subject).ThenInclude(s => s!.Grade)
            .Include(c => c.Category)
            .Include(c => c.Reviews)
            .Include(c => c.LiveSessions)
            .Include(c => c.Sections).ThenInclude(s => s.Lessons)
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<IReadOnlyList<Course>> GetByTeacherAsync(string teacherId, CancellationToken ct = default)
        => await _set.Where(c => c.TeacherId == teacherId).ToListAsync(ct);

    public async Task<IReadOnlyList<Course>> GetBySubjectAsync(int subjectId, CancellationToken ct = default)
        => await _set.Where(c => c.SubjectId == subjectId).ToListAsync(ct);

    public async Task<IReadOnlyList<Course>> GetPublishedAsync(CancellationToken ct = default)
        => await _set.Where(c => c.Status == CourseStatus.Published && c.ApprovalStatus == ApprovalStatus.Approved)
            .Include(c => c.Subject).ThenInclude(s => s!.Grade)
            .Include(c => c.Category)
            .Include(c => c.Reviews)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Course>> GetPendingApprovalAsync(CancellationToken ct = default)
        => await _set.Where(c => c.ApprovalStatus == ApprovalStatus.Pending).ToListAsync(ct);
}
