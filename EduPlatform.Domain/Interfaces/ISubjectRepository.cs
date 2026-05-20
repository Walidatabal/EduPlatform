using EduPlatform.Domain.Entities;

namespace EduPlatform.Domain.Interfaces;

public interface ISubjectRepository : IRepository<Subject>
{
    Task<IReadOnlyList<Subject>> GetByGradeAsync(int gradeId, CancellationToken ct = default);
}
