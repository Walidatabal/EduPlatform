using EduPlatform.Domain.Constants;
using EduPlatform.Domain.Entities;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders;

/// <summary>
/// Seeds the relation between parents and students.
///
/// Enterprise workflow supported:
/// Parent -> linked students -> enrollments/progress/certificates.
/// </summary>
public static class ParentStudentLinkSeeder
{
    public static async Task SeedAsync(
        AppDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (await context.ParentStudentLinks.IgnoreQueryFilters().AnyAsync())
            return;

        var parents = (await userManager.GetUsersInRoleAsync(AppRoles.Parent)).ToList();
        var students = (await userManager.GetUsersInRoleAsync(AppRoles.Student)).ToList();

        if (parents.Count == 0 || students.Count == 0)
            return;

        for (var i = 0; i < parents.Count; i++)
        {
            foreach (var student in students.Skip(i * 2).Take(2))
            {
                context.ParentStudentLinks.Add(new ParentStudentLink
                {
                    ParentId = parents[i].Id,
                    StudentId = student.Id,
                    RelationshipType = i == 0 ? "Father" : i == 1 ? "Mother" : "Guardian"
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
