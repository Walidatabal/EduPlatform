
using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace EduPlatform.Infrastructure.Seeders;

public static class StudentSeeder
{
    public static async Task SeedAsync(
        UserManager<ApplicationUser> userManager)
    {
        for (int i = 1; i <= 10; i++)
        {
            var email = $"student{i}@edu.com";

            if (await userManager.FindByEmailAsync(email) != null)
                continue;

            var student = new ApplicationUser
            {
                FullName = $"Student {i}",
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(student, "Student@123");

            await userManager.AddToRoleAsync(
                student,
                AppRoles.Student);
        }
    }
}