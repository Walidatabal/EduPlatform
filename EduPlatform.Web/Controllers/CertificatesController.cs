using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Web.ViewModels.Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// MVC controller for student certificates.
/// </summary>
[Authorize]
public class CertificatesController : Controller
{
    private readonly ICertificateService _certificateService;

    public CertificatesController(ICertificateService certificateService)
    {
        _certificateService = certificateService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var certificates = await _certificateService.GetCertificatesAsync(GetCurrentUserId(), ct);

        var vm = new CertificateIndexVM
        {
            Certificates = certificates.Select(c => new CertificateItemVM
            {
                Id = c.Id,
                CourseId = c.CourseId,
                CourseTitle = c.CourseTitle,
                CertificateNumber = c.CertificateNumber,
                IssuedAt = c.IssuedAt,
                Status = c.Status.ToString(),
                PdfUrl = c.PdfUrl
            }).ToList()
        };

        return View(vm);
    }

    private string GetCurrentUserId()
    {
        return User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("User is not authenticated.");
    }
}
