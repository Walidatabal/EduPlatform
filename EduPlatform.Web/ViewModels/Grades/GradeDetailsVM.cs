namespace EduPlatform.Web.ViewModels.Grades;
public class GradeDetailsVM
{
    public int    Id           { get; set; }
    public string Name         { get; set; } = string.Empty;
    public string? Description { get; set; }
    public IReadOnlyList<SubjectItemVM> Subjects { get; set; } = [];
}
public class SubjectItemVM
{
    public int    Id          { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string? Description{ get; set; }
    public int    CourseCount { get; set; }
}
