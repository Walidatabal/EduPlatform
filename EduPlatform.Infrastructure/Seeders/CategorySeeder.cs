using EduPlatform.Domain.Entities;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders;

public static class CategorySeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var categories = new List<Category>
        {
            new() { Name = "Programming" },
            new() { Name = "Web Development" },
            new() { Name = "AI & Machine Learning" },
            new() { Name = "Mobile Development" },
            new() { Name = "Database" }
        };

        await context.Categories.AddRangeAsync(categories);

        await context.SaveChangesAsync();
    }
}