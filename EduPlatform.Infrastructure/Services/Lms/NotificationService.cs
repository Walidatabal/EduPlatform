using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

public class NotificationService : INotificationService
{
    private readonly ILmsPlatformService _lms;

    public NotificationService(ILmsPlatformService lms) => _lms = lms;

    public Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(string userId, CancellationToken ct = default) => _lms.GetNotificationsAsync(userId, ct);
    public Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request, CancellationToken ct = default) => _lms.CreateNotificationAsync(request, ct);
    public Task MarkNotificationReadAsync(string userId, int id, CancellationToken ct = default) => _lms.MarkNotificationReadAsync(userId, id, ct);
}
