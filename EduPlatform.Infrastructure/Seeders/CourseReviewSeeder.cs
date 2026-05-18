using EduPlatform.Domain.Entities;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders;

public static class CourseReviewSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (await context.Set<CourseReview>().AnyAsync())
            return;

        var students = await userManager.GetUsersInRoleAsync("Student");
        if (!students.Any()) return;

        // OrderBy required before Take to avoid unpredictable SQL column ordering
        var courses = await context.Courses
            .OrderBy(x => x.Id)
            .Take(5)
            .ToListAsync();

        if (!courses.Any()) return;

        var reviews = new List<CourseReview>();

        foreach (var course in courses)
        {
            reviews.Add(new CourseReview
            {
                CourseId   = course.Id,
                StudentId  = students.First().Id,
                Rating     = 5,
                Comment    = "Excellent course.",
                IsApproved = true
            });
        }

        await context.Set<CourseReview>().AddRangeAsync(reviews);
        await context.SaveChangesAsync();
    }
}
