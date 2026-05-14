using EduPlatform.Domain.Entities;

namespace EduPlatform.Domain.Interfaces;

public interface IEnrollmentRepository : IRepository<Enrollment>
{
    Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, int courseId, CancellationToken ct = default);
    Task<IReadOnlyList<Enrollment>> GetByStudentAsync(string studentId, CancellationToken ct = default);
    Task<bool> IsEnrolledAsync(string studentId, int courseId, CancellationToken ct = default);
}
