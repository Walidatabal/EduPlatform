using EduPlatform.Application.Features.Lms.DTOs;

namespace EduPlatform.Application.Features.Lms.Interfaces;

public interface ILiveSessionService
{
    Task<IReadOnlyList<LiveSessionDto>> GetCourseLiveSessionsAsync(int courseId, CancellationToken ct = default);
    Task<string?> GetLiveSessionJoinUrlAsync(string userId, int sessionId, CancellationToken ct = default);
    Task<LiveSessionDto> CreateLiveSessionAsync(string instructorId, UpsertLiveSessionRequest request, CancellationToken ct = default);
    Task UpdateLiveSessionAsync(string userId, bool isAdmin, int id, UpsertLiveSessionRequest request, CancellationToken ct = default);

    Task<LiveSessionStatusDto> GoLiveAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default);
    Task<LiveSessionStatusDto> CompleteSessionAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default);
    Task CancelSessionAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default);

    Task<AttendanceDto> CheckInAsync(string studentId, int sessionId, CancellationToken ct = default);
    Task<AttendanceDto> CheckOutAsync(string studentId, int sessionId, CancellationToken ct = default);
    Task<SessionAttendanceSummaryDto> GetAttendanceSummaryAsync(string userId, bool isAdmin, int sessionId, CancellationToken ct = default);
    Task<AttendanceDto> UpdateAttendanceAsync(string userId, bool isAdmin, int attendanceId, UpdateAttendanceRequest request, CancellationToken ct = default);
    Task<IReadOnlyList<AttendanceDto>> GetMyAttendanceAsync(string studentId, CancellationToken ct = default);
}
