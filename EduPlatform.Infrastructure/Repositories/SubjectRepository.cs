using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Interfaces;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Repositories;

public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(AppDbContext db) : base(db) { }

    public async Task<IReadOnlyList<Subject>> GetByGradeAsync(int gradeId, CancellationToken ct = default)
        => await _set.Where(s => s.GradeId == gradeId).ToListAsync(ct);
}
