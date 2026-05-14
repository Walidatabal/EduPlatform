using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace EduPlatform.Infrastructure.Seeders;

public static class AdminSeeder
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager, IConfiguration config)
    {
        var email    = config["Seeding:AdminEmail"]    ?? "admin@eduplatform.com";
        var password = config["Seeding:AdminPassword"] ?? throw new InvalidOperationException("Seeding:AdminPassword not configured.");
        var fullName = config["Seeding:AdminFullName"] ?? "Platform Admin";

        if (await userManager.FindByEmailAsync(email) is not null) return;

        var user = new ApplicationUser { UserName = email, Email = email, FullName = fullName, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
            await userManager.AddToRoleAsync(user, AppRoles.Admin);
    }
}
