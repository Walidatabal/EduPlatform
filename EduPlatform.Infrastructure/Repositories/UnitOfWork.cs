using EduPlatform.Domain.Interfaces;
using EduPlatform.Infrastructure.Data;

namespace EduPlatform.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;
    public IGradeRepository Grades { get; }
    public ISubjectRepository Subjects { get; }
    public ICourseRepository Courses { get; }
    public IEnrollmentRepository Enrollments { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Grades = new GradeRepository(db);
        Subjects = new SubjectRepository(db);
        Courses = new CourseRepository(db);
        Enrollments = new EnrollmentRepository(db);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _db.SaveChangesAsync(ct);

    public void Dispose() => _db.Dispose();
}
