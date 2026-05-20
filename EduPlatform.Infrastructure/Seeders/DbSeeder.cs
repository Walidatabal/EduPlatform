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

        const int maxAttempts = 10;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                var hasMigrations = db.Database.GetMigrations().Any();

                if (hasMigrations)
                {
                    logger.LogInformation("Applying EF migrations. Attempt {A}/{Max}", attempt, maxAttempts);
                    await db.Database.MigrateAsync();
                }
                else
                {
                    logger.LogWarning("No EF migrations found — using EnsureCreatedAsync (demo/local mode).");
                    await db.Database.EnsureCreatedAsync();
                }

                break;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                logger.LogWarning(ex, "Database not ready. Retrying in 5 s…");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        var roleManager = sp.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = sp.GetRequiredService<UserManager<ApplicationUser>>();
        var config = sp.GetRequiredService<IConfiguration>();

        // ── Core identity ──────────────────────────────────────────────────────
        await RoleSeeder.SeedAsync(roleManager);
        await AdminSeeder.SeedAsync(userManager, config);

        // ── Academic structure ─────────────────────────────────────────────────
        await CategorySeeder.SeedAsync(db);
        await GradeSeeder.SeedAsync(db);
        await SubjectSeeder.SeedAsync(db);

        // ── Users ──────────────────────────────────────────────────────────────
        await TeacherSeeder.SeedAsync(userManager);
        await StudentSeeder.SeedAsync(userManager);
        await ParentSeeder.SeedAsync(userManager);

        // ── Courses & curriculum ───────────────────────────────────────────────
        await CourseSeeder.SeedAsync(db, userManager);
        await SectionLessonSeeder.SeedAsync(db);

        // ── Activity ───────────────────────────────────────────────────────────
        await EnrollmentSeeder.SeedAsync(db, userManager);
        await CourseReviewSeeder.SeedAsync(db, userManager);

        // ── Relations ─────────────────────────────────────────────────────────
        await ParentStudentLinkSeeder.SeedAsync(db, userManager);

        // ── Advanced LMS demo data ─────────────────────────────────────────────
        await LmsDemoSeeder.SeedAsync(db, userManager);

        // ── Attendance test data ───────────────────────────────────────────────
        await AttendanceSeeder.SeedAsync(db, userManager);
    }
}
