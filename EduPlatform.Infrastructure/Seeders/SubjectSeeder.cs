using EduPlatform.Domain.Entities;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders;

public static class SubjectSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Set<Subject>().AnyAsync())
            return;

        var grades = await context.Set<Grade>().ToListAsync();

        if (!grades.Any())
            return;

        var subjects = new List<Subject>
        {
            new()
            {
                Name = "ASP.NET Core",
                GradeId = grades.First().Id
            },

            new()
            {
                Name = "Entity Framework Core",
                GradeId = grades.First().Id
            },

            new()
            {
                Name = "SQL Server",
                GradeId = grades.Last().Id
            }
        };

        await context.Set<Subject>().AddRangeAsync(subjects);

        await context.SaveChangesAsync();
    }
}