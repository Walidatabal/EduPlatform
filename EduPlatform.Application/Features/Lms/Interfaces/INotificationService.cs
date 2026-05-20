using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface INotificationService
{
    Task<IReadOnlyList<NotificationDto>> GetNotificationsAsync(string userId, CancellationToken ct = default);
    Task<NotificationDto> CreateNotificationAsync(CreateNotificationRequest request, CancellationToken ct = default);
    Task MarkNotificationReadAsync(string userId, int id, CancellationToken ct = default);
}
