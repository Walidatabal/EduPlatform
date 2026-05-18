using EduPlatform.Domain.Constants;
using Microsoft.AspNetCore.Identity;

namespace EduPlatform.Infrastructure.Seeders;

public static class RoleSeeder
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { 
            AppRoles.Admin, 
            AppRoles.Teacher, 
            AppRoles.PendingTeacher,
            AppRoles.Student, 
            AppRoles.Parent, 
            AppRoles.ContentManager };
        foreach (var role in roles)
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
    }
}
