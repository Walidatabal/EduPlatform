using EduPlatform.Domain.Entities;

namespace EduPlatform.Domain.Interfaces;

public interface IGradeRepository : IRepository<Grade>
{
    Task<Grade?> GetWithSubjectsAsync(int id, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken ct = default);
}
