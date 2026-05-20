namespace EduPlatform.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default);

    Task SendWelcomeAsync(
        string to,
        string name,
        CancellationToken cancellationToken = default);

    Task SendEnrollmentConfirmationAsync(
        string to,
        string courseName,
        CancellationToken cancellationToken = default);

    Task SendTeacherApprovedAsync(
        string to,
        string teacherName,
        CancellationToken cancellationToken = default);
}
