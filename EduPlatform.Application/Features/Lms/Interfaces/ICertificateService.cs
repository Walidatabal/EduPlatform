using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface ICertificateService
{
    Task<IReadOnlyList<CertificateDto>> GetCertificatesAsync(string studentId, CancellationToken ct = default);
    Task<CertificateDto> IssueCertificateAsync(string studentId, int courseId, CancellationToken ct = default);
    Task RevokeCertificateAsync(int id, CancellationToken ct = default);
}
