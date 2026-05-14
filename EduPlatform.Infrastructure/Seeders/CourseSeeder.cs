using EduPlatform.Domain.Constants;
using EduPlatform.Domain.Entities;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders
{
    public static class CourseSeeder
    {
        public static async Task SeedAsync(
            AppDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            if (context.Courses.Any())
                return;

            var teachers = await userManager.GetUsersInRoleAsync(AppRoles.Teacher);
            if (!teachers.Any()) return;

            var categories = await context.Categories.ToListAsync();
            if (!categories.Any()) return;

            // SubjectId is a required FK (non-nullable int) — must be set or SaveChanges throws
            var subjects = await context.Set<Subject>().ToListAsync();
            if (!subjects.Any()) return;

            var courses = new List<Course>();
            int counter = 1;

            foreach (var teacher in teachers)
            {
                foreach (var category in categories.Take(2))
                {
                    // Round-robin subjects so every course gets a valid SubjectId
                    var subject = subjects[(counter - 1) % subjects.Count];

                    courses.Add(new Course
                    {
                        Title       = $"Course {counter}",
                        Description = "Professional course description",
                        Price       = 49.00m,          // decimal literal — matches HasPrecision(18,2)
                        TeacherId   = teacher.Id,
                        CategoryId  = category.Id,
                        SubjectId   = subject.Id,       // required FK — was missing, caused crash
                        Level       = "Beginner",       // required (HasMaxLength + IsRequired)
                        Language    = "English",        // required (HasMaxLength + IsRequired)
                        CreatedAt   = DateTime.UtcNow
                    });

                    counter++;
                }
            }

            await context.Courses.AddRangeAsync(courses);
            await context.SaveChangesAsync();
        }
    }
}
