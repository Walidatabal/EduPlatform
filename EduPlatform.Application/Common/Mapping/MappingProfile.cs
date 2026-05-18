using AutoMapper;
using EduPlatform.Application.Features.Courses.DTOs;
using EduPlatform.Application.Features.Grades.DTOs;
using EduPlatform.Application.Features.Lessons.DTOs;
using EduPlatform.Application.Features.Sections.DTOs;
using EduPlatform.Application.Features.Subjects.DTOs;
using EduPlatform.Domain.Entities;

namespace EduPlatform.Application.Common.Mapping;

/// <summary>
/// AutoMapper configuration profile for the Application layer.
///
/// What is AutoMapper?
/// AutoMapper eliminates repetitive object-to-object mapping code.
/// Without it, mapping a Course to CourseDto requires manually copying
/// each property (10+ lines). With it: _mapper.Map&lt;CourseDto&gt;(course).
///
/// How maps work:
/// CreateMap&lt;Source, Destination&gt;() tells AutoMapper which types to map.
/// Properties with matching names map automatically (convention-based).
/// Use .ForMember() to handle mismatched names, computed values, or null-safe navigation.
///
/// Registration:
/// This profile is registered in ApplicationServiceRegistration.cs:
///   services.AddAutoMapper(typeof(MappingProfile).Assembly);
/// AutoMapper scans the assembly and registers every Profile subclass automatically.
///
/// Rule: Maps in this profile are for entity → DTO only (read direction).
/// Command → Entity mapping is handled by service methods directly.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ── Grade → GradeDto ───────────────────────────────────────────────────
        // All properties match by name: Id, Name, Description, CreatedAt.
        // GradeDto is the safe read model — never exposes IsDeleted or audit fields.
        CreateMap<Grade, GradeDto>();

        // ── Subject → SubjectDto ───────────────────────────────────────────────
        // SubjectDto includes GradeId for client-side filtering.
        // GradeName is not on SubjectDto — load Grade separately if needed.
        CreateMap<Subject, SubjectDto>();

        // ── Course → CourseDto (detail view) ──────────────────────────────────
        // CourseDto is used on the course detail page.
        // It includes nested Sections and Lessons.
        // AutoMapper maps Sections collection automatically because both
        // Course.Sections and CourseDto.Sections have matching collection types.
        CreateMap<Course, CourseDto>();

        // ── Course → CourseListDto (catalog view) ─────────────────────────────
        // CourseListDto is a lighter model for the course catalog list.
        // It does not include Sections (no curriculum preview in the list).
        // AverageRating and ReviewCount are computed in the DTO if Reviews are loaded.
        CreateMap<Course, CourseListDto>();

        // ── Section → SectionDto ──────────────────────────────────────────────
        // Includes the ordered Lessons collection for curriculum display.
        CreateMap<Section, SectionDto>();

        // ── Lesson → LessonDto ────────────────────────────────────────────────
        // LessonDto exposes Title, ContentType, DurationSeconds, IsFreePreview.
        // VideoUrl is only included if the student is enrolled (filtered in service).
        CreateMap<Lesson, LessonDto>();
    }
}
