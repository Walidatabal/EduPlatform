using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EduPlatform.Infrastructure.Seeders;

public static class DbSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("DbSeeder");

        var db = sp.GetRequiredService<AppDbContext>();

        // Docker SQL Server can be healthy slightly later than the API startup.
        // This retry prevents repeated API crashes during first startup.
        const int maxAttempts = 10;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                logger.LogInformation("Applying database migrations. Attempt {Attempt}/{MaxAttempts}", attempt, maxAttempts);
                await db.Database.MigrateAsync();
                break;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(ex, "Database is not ready yet. Retrying in 5 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var config = sp.GetRequiredService<IConfiguration>();

        await RoleSeeder.SeedAsync(roleManager);
        await AdminSeeder.SeedAsync(userManager, config);


        // Demo/Test data


        await CategorySeeder.SeedAsync(db);
        await GradeSeeder.SeedAsync(db);
        await SubjectSeeder.SeedAsync(db);

        await TeacherSeeder.SeedAsync(userManager);
        await StudentSeeder.SeedAsync(userManager);

        await CourseSeeder.SeedAsync(db, userManager);
        await EnrollmentSeeder.SeedAsync(db, userManager);
        await CourseReviewSeeder.SeedAsync(db, userManager);
    }
}
