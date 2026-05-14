using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface ILiveSessionService
{
    Task<IReadOnlyList<LiveSessionDto>> GetCourseLiveSessionsAsync(int courseId, CancellationToken ct = default);
    Task<string?> GetLiveSessionJoinUrlAsync(string userId, int sessionId, CancellationToken ct = default);
    Task<LiveSessionDto> CreateLiveSessionAsync(string instructorId, UpsertLiveSessionRequest request, CancellationToken ct = default);
    Task UpdateLiveSessionAsync(string userId, bool isAdmin, int id, UpsertLiveSessionRequest request, CancellationToken ct = default);
}
