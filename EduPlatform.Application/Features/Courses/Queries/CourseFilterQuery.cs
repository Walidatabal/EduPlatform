namespace EduPlatform.Application.Features.Courses.Queries;

public class CourseFilterQuery
{
    public int? GradeId { get; set; }
    public int? SubjectId { get; set; }
    public int? CategoryId { get; set; }
    public string? Level { get; set; }
    public bool? IsFree { get; set; }
    public string? Search { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "asc";
}
