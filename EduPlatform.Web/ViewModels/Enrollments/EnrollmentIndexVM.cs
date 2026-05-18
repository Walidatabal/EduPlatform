namespace EduPlatform.Web.ViewModels.Enrollments;

/// <summary>
/// My Learning / Enrollments page ViewModel.
/// </summary>
public class EnrollmentIndexVM
{
    public List<EnrollmentItemVM> Enrollments { get; set; } = [];
}

public class EnrollmentItemVM
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime EnrolledAt { get; set; }
    public decimal AmountPaid { get; set; }
}
