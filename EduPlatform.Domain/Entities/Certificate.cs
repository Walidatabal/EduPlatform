using EduPlatform.Domain.Common;
using EduPlatform.Domain.Enums;

namespace EduPlatform.Domain.Entities;

public class Certificate : BaseEntity
{
    public string StudentId { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public Course? Course { get; set; }
    public string CertificateNumber { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; }
    public CertificateStatus Status { get; set; } = CertificateStatus.Issued;
    public string? PdfUrl { get; set; }
}
