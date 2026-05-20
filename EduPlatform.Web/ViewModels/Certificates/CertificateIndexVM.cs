namespace EduPlatform.Web.ViewModels.Certificates;

/// <summary>
/// Student certificates page ViewModel.
/// </summary>
public class CertificateIndexVM
{
    public List<CertificateItemVM> Certificates { get; set; } = [];
}

public class CertificateItemVM
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? PdfUrl { get; set; }
}
