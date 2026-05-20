using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Common.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EduPlatform.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public EmailService(IOptions<EmailSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        var email = new MimeMessage();

        email.From.Add(
            MailboxAddress.Parse(_settings.From));

        email.To.Add(
            MailboxAddress.Parse(to));

        email.Subject = subject;

        email.Body = new TextPart("html")
        {
            Text = body
        };

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(
            _settings.Host,
            _settings.Port,
            MailKit.Security.SecureSocketOptions.StartTls,
            cancellationToken);

        await smtp.AuthenticateAsync(
            _settings.Username,
            _settings.Password,
            cancellationToken);

        await smtp.SendAsync(
            email,
            cancellationToken);

        await smtp.DisconnectAsync(
            true,
            cancellationToken);
    }

    public async Task SendWelcomeAsync(
        string to,
        string name,
        CancellationToken cancellationToken = default)
    {
        var body = $"""
            <h1>Welcome {name}</h1>
            <p>Welcome to EduPlatform.</p>
            """;

        await SendAsync(
            to,
            "Welcome to EduPlatform",
            body,
            cancellationToken);
    }

    public async Task SendEnrollmentConfirmationAsync(
        string to,
        string courseName,
        CancellationToken cancellationToken = default)
    {
        var body = $"""
            <h1>Enrollment Successful</h1>
            <p>You enrolled in {courseName}</p>
            """;

        await SendAsync(
            to,
            "Course Enrollment",
            body,
            cancellationToken);
    }

    public async Task SendTeacherApprovedAsync(
        string to,
        string teacherName,
        CancellationToken cancellationToken = default)
    {
        var body = $"""
            <h1>Teacher Approved</h1>
            <p>Congratulations {teacherName}, your account has been approved.</p>
            """;

        await SendAsync(
            to,
            "Teacher Approval",
            body,
            cancellationToken);
    }
}