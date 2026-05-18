namespace EduPlatform.Web.ViewModels.Courses;
public class CourseDetailsVM
{
    public int     Id               { get; set; }
    public string  Title            { get; set; } = string.Empty;
    public string? Description      { get; set; }
    public string? Requirements     { get; set; }
    public string? LearningOutcomes { get; set; }
    public string  Level            { get; set; } = string.Empty;
    public string  Language         { get; set; } = string.Empty;
    public decimal Price            { get; set; }
    public string? CategoryName     { get; set; }
    public string? SubjectName      { get; set; }
    public string? GradeName        { get; set; }
    public string  Status           { get; set; } = string.Empty;
    public double  AverageRating    { get; set; }
    public int     ReviewCount      { get; set; }
    public int     EnrollmentCount  { get; set; }
    public bool    IsEnrolled       { get; set; }
    public IReadOnlyList<CourseSectionVM> Sections  { get; set; } = [];
    public IReadOnlyList<CourseReviewVM>  Reviews   { get; set; } = [];
}
public class CourseSectionVM
{
    public int    Order    { get; set; }
    public string Title    { get; set; } = string.Empty;
    public IReadOnlyList<CourseLessonVM> Lessons { get; set; } = [];
}
public class CourseLessonVM
{
    public int    Order       { get; set; }
    public string Title       { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
}
public class CourseReviewVM
{
    public string StudentName { get; set; } = string.Empty;
    public int    Rating      { get; set; }
    public string? Comment    { get; set; }
}
