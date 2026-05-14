namespace EduPlatform.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default);
    Task SendWelcomeAsync(string to, string userName, CancellationToken ct = default);
    Task SendEnrollmentConfirmationAsync(string to, string courseName, CancellationToken ct = default);
    Task SendTeacherApprovedAsync(string to, string teacherName, CancellationToken ct = default);
}
