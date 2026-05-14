using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace EduPlatform.Infrastructure.Seeders;

public static class TeacherSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager)
    {
        for (int i = 1; i <= 3; i++)
        {
            var email = $"teacher{i}@edu.com";

            if (await userManager.FindByEmailAsync(email) != null)
                continue;

            var teacher = new ApplicationUser
            {
                FullName = $"Teacher {i}",
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(teacher, "Teacher@123");

            await userManager.AddToRoleAsync(
                teacher,
                AppRoles.Teacher);
        }
    }
}