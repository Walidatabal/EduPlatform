using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

public class CertificateService : ICertificateService
{
    private readonly ILmsPlatformService _lms;

    public CertificateService(ILmsPlatformService lms) => _lms = lms;

    public Task<IReadOnlyList<CertificateDto>> GetCertificatesAsync(string studentId, CancellationToken ct = default) => _lms.GetCertificatesAsync(studentId, ct);
    public Task<CertificateDto> IssueCertificateAsync(string studentId, int courseId, CancellationToken ct = default) => _lms.IssueCertificateAsync(studentId, courseId, ct);
    public Task RevokeCertificateAsync(int id, CancellationToken ct = default) => _lms.RevokeCertificateAsync(id, ct);
}
