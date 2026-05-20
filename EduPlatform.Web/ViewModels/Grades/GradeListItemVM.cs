namespace EduPlatform.Web.ViewModels.Grades;
public class GradeListItemVM
{
    public int    Id           { get; set; }
    public string Name         { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int    SubjectCount { get; set; }
    public int    CourseCount  { get; set; }
}
