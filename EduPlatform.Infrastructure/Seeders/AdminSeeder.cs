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

        var user = await userManager.FindByEmailAsync(email);

        if (user is null)
        {
            user = new ApplicationUser
            {
                UserName        = email,
                Email           = email,
                FullName        = fullName,
                EmailConfirmed  = true,
                LockoutEnabled  = false,
                LockoutEnd      = null,
                CreatedAt       = DateTime.UtcNow
            };

            var createResult = await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Admin seeding failed: " + string.Join(" | ", createResult.Errors.Select(e => e.Description)));
            }
        }
        else
        {
            // Keep the seeded admin deterministic for local/Docker testing.
            // This fixes the common issue where the admin existed with an old password or was locked out.
            user.UserName        = email;
            user.Email           = email;
            user.FullName        = string.IsNullOrWhiteSpace(user.FullName) ? fullName : user.FullName;
            user.EmailConfirmed  = true;
            user.LockoutEnabled  = false;
            user.LockoutEnd      = null;
            user.AccessFailedCount = 0;

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Admin update failed: " + string.Join(" | ", updateResult.Errors.Select(e => e.Description)));
            }

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await userManager.ResetPasswordAsync(user, resetToken, password);

            if (!resetResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Admin password reset failed: " + string.Join(" | ", resetResult.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, AppRoles.Admin))
        {
            var roleResult = await userManager.AddToRoleAsync(user, AppRoles.Admin);
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(
                    "Admin role assignment failed: " + string.Join(" | ", roleResult.Errors.Select(e => e.Description)));
            }
        }
    }
}
