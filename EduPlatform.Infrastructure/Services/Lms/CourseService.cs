using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

/// <summary>
/// Placeholder implementation for ICourseService.
/// 
/// ICourseService is reserved for future course-management operations that will
/// be extracted from ILmsPlatformService as the platform grows. Today, all course
/// CRUD lives in CoursesController (API) and CoursesController (Web) via IUnitOfWork.
/// 
/// This class satisfies the DI registration without any runtime overhead.
/// </summary>
public class CourseService : ICourseService
{
    // No members required until operations are moved here from LmsPlatformService.
    // When you add the first method, also update ICourseService and DependencyInjection.cs.
}
