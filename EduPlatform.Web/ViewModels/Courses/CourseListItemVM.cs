namespace EduPlatform.Web.ViewModels.Courses;
public class CourseListItemVM
{
    public int     Id              { get; set; }
    public string  Title           { get; set; } = string.Empty;
    public string? Description     { get; set; }
    public string  Level           { get; set; } = string.Empty;
    public string  Language        { get; set; } = string.Empty;
    public decimal Price           { get; set; }
    public string? ThumbnailUrl    { get; set; }   // ← NEW
    public string? CategoryName    { get; set; }
    public string? SubjectName     { get; set; }
    public string? GradeName       { get; set; }
    public int     ReviewCount     { get; set; }
    public double  AverageRating   { get; set; }
    public int     EnrollmentCount { get; set; }
}
