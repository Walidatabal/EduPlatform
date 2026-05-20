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

    public Task<LiveSessionStatusDto> GoLiveAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default) => _lms.GoLiveAsync(userId, isAdmin, sessionId, ct);
    public Task<LiveSessionStatusDto> CompleteSessionAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default) => _lms.CompleteSessionAsync(userId, isAdmin, sessionId, ct);
    public Task CancelSessionAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default) => _lms.CancelSessionAsync(userId, isAdmin, sessionId, ct);

    public Task<AttendanceDto> CheckInAsync(string studentId, int sessionId, CancellationToken ct = default) => _lms.CheckInAsync(studentId, sessionId, ct);
    public Task<AttendanceDto> CheckOutAsync(string studentId, int sessionId, CancellationToken ct = default) => _lms.CheckOutAsync(studentId, sessionId, ct);
    public Task<SessionAttendanceSummaryDto> GetAttendanceSummaryAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default) => _lms.GetAttendanceSummaryAsync(userId, isAdmin, sessionId, ct);
    public Task<AttendanceDto> UpdateAttendanceAsync(string userId, bool isAdmin, int attendanceId, UpdateAttendanceRequest request, CancellationToken ct = default) => _lms.UpdateAttendanceAsync(userId, isAdmin, attendanceId, request, ct);
    public Task<IReadOnlyList<AttendanceDto>> GetMyAttendanceAsync(string studentId, CancellationToken ct = default) => _lms.GetMyAttendanceAsync(studentId, ct);
}
