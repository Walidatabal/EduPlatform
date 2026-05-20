using EduPlatform.Domain.Entities;
using EduPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders;

/// <summary>
/// Seeds course curriculum content for testing the learning flow.
/// 
/// Enterprise purpose:
/// - Courses should not be empty.
/// - Student dashboard/progress needs real sections and lessons.
/// - Course details page can show a realistic curriculum.
/// </summary>
public static class SectionLessonSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (await context.Sections.IgnoreQueryFilters().AnyAsync())
            return;

        var courses = await context.Courses
            .OrderBy(c => c.Id)
            .ToListAsync();

        if (courses.Count == 0)
            return;

        var sections = new List<Section>();

        foreach (var course in courses)
        {
            sections.Add(new Section
            {
                CourseId = course.Id,
                Title = "Getting Started",
                Order = 1,
                Lessons =
                {
                    new Lesson
                    {
                        Title = "Course Introduction",
                        Order = 1,
                        DurationSeconds = 420,
                        IsFreePreview = true,
                        ContentType = "Video",
                        VideoUrl = "https://example.com/videos/intro"
                    },
                    new Lesson
                    {
                        Title = "How to Use This Course",
                        Order = 2,
                        DurationSeconds = 600,
                        IsFreePreview = true,
                        ContentType = "Article",
                        ArticleHtml = "<p>This lesson explains how to follow the course professionally.</p>"
                    }
                }
            });

            sections.Add(new Section
            {
                CourseId = course.Id,
                Title = "Core Lessons",
                Order = 2,
                Lessons =
                {
                    new Lesson
                    {
                        Title = "Main Concept",
                        Order = 1,
                        DurationSeconds = 900,
                        IsFreePreview = false,
                        ContentType = "Video",
                        VideoUrl = "https://example.com/videos/main-concept"
                    },
                    new Lesson
                    {
                        Title = "Practical Exercise",
                        Order = 2,
                        DurationSeconds = 720,
                        IsFreePreview = false,
                        ContentType = "Video",
                        ResourceUrl = "https://example.com/resources/exercise.pdf"
                    }
                }
            });
        }

        await context.Sections.AddRangeAsync(sections);
        await context.SaveChangesAsync();
    }
}
