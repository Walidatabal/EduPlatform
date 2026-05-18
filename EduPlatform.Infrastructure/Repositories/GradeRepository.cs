using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Interfaces;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Repositories;

public class GradeRepository : BaseRepository<Grade>, IGradeRepository
{
    public GradeRepository(AppDbContext db) : base(db) { }

    public async Task<Grade?> GetWithSubjectsAsync(int id, CancellationToken ct = default)
        => await _set.Include(g => g.Subjects).FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken ct = default)
        => await _set.AnyAsync(g => g.Name == name && (excludeId == null || g.Id != excludeId), ct);
}
