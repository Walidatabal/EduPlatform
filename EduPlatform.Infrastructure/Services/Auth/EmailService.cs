using EduPlatform.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EduPlatform.Infrastructure.Services.Auth;

// Stub implementation - swap for SendGrid/SMTP in production
public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
    {
        _logger.LogInformation("[EMAIL] To: {To} | Subject: {Subject}", to, subject);
        return Task.CompletedTask;
    }

    public Task SendWelcomeAsync(string to, string userName, CancellationToken ct = default)
        => SendAsync(to, "Welcome to EduPlatform!", $"<h1>Welcome, {userName}!</h1>", ct);

    public Task SendEnrollmentConfirmationAsync(string to, string courseName, CancellationToken ct = default)
        => SendAsync(to, "Enrollment Confirmed", $"<p>You are now enrolled in <b>{courseName}</b>.</p>", ct);

    public Task SendTeacherApprovedAsync(string to, string teacherName, CancellationToken ct = default)
        => SendAsync(to, "Teacher Account Approved", $"<p>Congratulations {teacherName}, your teacher account has been approved!</p>", ct);
}
