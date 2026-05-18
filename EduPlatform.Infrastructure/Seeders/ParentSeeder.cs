using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace EduPlatform.Infrastructure.Seeders;

/// <summary>
/// Seeds demo parent accounts used to test parent dashboards and student monitoring flows.
/// </summary>
public static class ParentSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        for (int i = 1; i <= 3; i++)
        {
            var email = $"parent{i}@edu.com";

            if (await userManager.FindByEmailAsync(email) is not null)
                continue;

            var parent = new ApplicationUser
            {
                FullName = $"Parent {i}",
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(parent, "Parent@123");
            await userManager.AddToRoleAsync(parent, AppRoles.Parent);
        }
    }
}
