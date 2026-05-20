using EduPlatform.Domain.Entities;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders;

public static class EnrollmentSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (await context.Enrollments.IgnoreQueryFilters().AnyAsync())
            return;

        var students = await userManager.GetUsersInRoleAsync("Student");
        if (!students.Any()) return;

        // OrderBy required before Take to avoid unpredictable SQL results
        var courses = await context.Courses
            .OrderBy(x => x.Id)
            .Take(5)
            .ToListAsync();

        if (!courses.Any()) return;

        var enrollments = new List<Enrollment>();

        // Track (studentId, courseId) pairs to prevent duplicate inserts
        // which would violate the unique index IX_Enrollments_StudentId_CourseId
        var seen = new HashSet<(string, int)>();

        foreach (var student in students)
        {
            foreach (var course in courses.Take(2))
            {
                var key = (student.Id, course.Id);
                if (!seen.Add(key)) continue;  // skip if already added

                enrollments.Add(new Enrollment
                {
                    StudentId   = student.Id,
                    CourseId    = course.Id,
                    AmountPaid  = course.Price,
                    EnrolledAt = DateTime.UtcNow
                });
            }
        }

        if (enrollments.Any())
        {
            await context.Enrollments.AddRangeAsync(enrollments);
            await context.SaveChangesAsync();
        }
    }
}
