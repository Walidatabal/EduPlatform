using EduPlatform.Domain.Interfaces;

namespace EduPlatform.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGradeRepository Grades { get; }
    ISubjectRepository Subjects { get; }
    ICourseRepository Courses { get; }
    IEnrollmentRepository Enrollments { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
