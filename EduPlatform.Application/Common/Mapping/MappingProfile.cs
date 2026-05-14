using AutoMapper;
using EduPlatform.Application.Features.Courses.DTOs;
using EduPlatform.Application.Features.Grades.DTOs;
using EduPlatform.Application.Features.Lessons.DTOs;
using EduPlatform.Application.Features.Sections.DTOs;
using EduPlatform.Application.Features.Subjects.DTOs;
using EduPlatform.Domain.Entities;

namespace EduPlatform.Application.Common.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Grade, GradeDto>();
        CreateMap<Subject, SubjectDto>();
        CreateMap<Course, CourseDto>();
        CreateMap<Course, CourseListDto>();
        CreateMap<Section, SectionDto>();
        CreateMap<Lesson, LessonDto>();
    }
}
