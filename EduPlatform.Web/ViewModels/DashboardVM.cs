// Kept for backward compatibility — new ViewModel is ViewModels/Dashboard/DashboardVM.cs
namespace EduPlatform.Web.ViewModels;
public class DashboardVM
{
    public int GradesCount      { get; set; }
    public int SubjectsCount    { get; set; }
    public int CoursesCount     { get; set; }
    public int EnrollmentsCount { get; set; }
}
