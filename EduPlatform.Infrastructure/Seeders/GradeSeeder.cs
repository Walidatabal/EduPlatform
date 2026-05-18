using EduPlatform.Domain.Entities;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders;

public static class GradeSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Set<Grade>().AnyAsync())
            return;

        var grades = new List<Grade>
        {
            new() { Name = "Beginner" },
            new() { Name = "Intermediate" },
            new() { Name = "Advanced" }
        };

        await context.Set<Grade>().AddRangeAsync(grades);

        await context.SaveChangesAsync();
    }
}