using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Interfaces;

public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetWithDetailsAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetByTeacherAsync(string teacherId, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetBySubjectAsync(int subjectId, CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetPublishedAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Course>> GetPendingApprovalAsync(CancellationToken ct = default);
}
