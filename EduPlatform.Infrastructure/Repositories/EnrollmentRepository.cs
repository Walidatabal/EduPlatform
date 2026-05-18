using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Interfaces;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Repositories;

public class EnrollmentRepository : BaseRepository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(AppDbContext db) : base(db) { }

    public async Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, int courseId, CancellationToken ct = default)
        => await _set.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);

    public async Task<IReadOnlyList<Enrollment>> GetByStudentAsync(string studentId, CancellationToken ct = default)
        => await _set.Where(e => e.StudentId == studentId).Include(e => e.Course).ToListAsync(ct);

    public async Task<bool> IsEnrolledAsync(string studentId, int courseId, CancellationToken ct = default)
        => await _set.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, ct);
}
