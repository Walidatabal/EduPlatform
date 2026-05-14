using EduPlatform.Application.Features.Lms.DTOs;
using EduPlatform.Application.Features.Lms.Interfaces;

namespace EduPlatform.Infrastructure.Services.Lms;

public class LiveSessionService : ILiveSessionService
{
    private readonly ILmsPlatformService _lms;

    public LiveSessionService(ILmsPlatformService lms) => _lms = lms;

    public Task<IReadOnlyList<LiveSessionDto>> GetCourseLiveSessionsAsync(int courseId, CancellationToken ct = default) => _lms.GetCourseLiveSessionsAsync(courseId, ct);
    public Task<string?> GetLiveSessionJoinUrlAsync(string userId, int sessionId, CancellationToken ct = default) => _lms.GetLiveSessionJoinUrlAsync(userId, sessionId, ct);
    public Task<LiveSessionDto> CreateLiveSessionAsync(string instructorId, UpsertLiveSessionRequest request, CancellationToken ct = default) => _lms.CreateLiveSessionAsync(instructorId, request, ct);
    public Task UpdateLiveSessionAsync(string userId, bool isAdmin, int id, UpsertLiveSessionRequest request, CancellationToken ct = default) => _lms.UpdateLiveSessionAsync(userId, isAdmin, id, request, ct);
}
