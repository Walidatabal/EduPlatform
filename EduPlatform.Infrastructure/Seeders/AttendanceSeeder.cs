using EduPlatform.Domain.Entities;
using EduPlatform.Domain.Enums;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EduPlatform.Infrastructure.Seeders;

/// <summary>
/// Seeds realistic attendance data for testing.
/// Creates:
///   - 2 Live sessions per course (1 Completed, 1 Live)
///   - Students registered with varied statuses (Present / Late / Absent)
///   - JoinedAt / LeftAt / DurationMinutes populated realistically
/// </summary>
public static class AttendanceSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        // Only seed if no attendance records exist
        if (await db.SessionAttendances.IgnoreQueryFilters().AnyAsync())
            return;

        var students = (await userManager.GetUsersInRoleAsync("Student")).ToList();
        var teachers = (await userManager.GetUsersInRoleAsync("Teacher")).ToList();
        var courses = await db.Courses.OrderBy(c => c.Id).Take(4).ToListAsync();

        if (students.Count == 0 || teachers.Count == 0 || courses.Count == 0)
            return;

        // Clear existing live sessions seeded by LmsDemoSeeder so we can
        // replace them with ones that have attendance data
        var existingSessions = await db.LiveSessions.IgnoreQueryFilters().ToListAsync();
        if (existingSessions.Any())
        {
            db.LiveSessions.RemoveRange(existingSessions);
            await db.SaveChangesAsync();
        }

        var now = DateTime.UtcNow;
        var rng = new Random(42); // fixed seed for reproducibility

        foreach (var course in courses)
        {
            var instructorId = !string.IsNullOrWhiteSpace(course.TeacherId)
                ? course.TeacherId
                : teachers.First().Id;

            // ── Session 1: COMPLETED (happened yesterday) ────────────────────
            var completedSession = new LiveSession
            {
                CourseId = course.Id,
                InstructorId = instructorId,
                Title = $"Week 1 Review — {course.Title}",
                Description = "Recap of week 1 topics and Q&A.",
                StartTime = now.AddDays(-1).Date.AddHours(10),
                EndTime = now.AddDays(-1).Date.AddHours(11),
                MeetingUrl = "https://meet.example.com/session-w1",
                Status = LiveSessionStatus.Completed,
                MaxStudents = 30,
                IsRecorded = true,
                RecordingUrl = "https://recordings.example.com/week1"
            };

            // Enroll first 6 students with varied statuses
            var sessionStudents = students.Take(6).ToList();
            foreach (var (student, index) in sessionStudents.Select((s, i) => (s, i)))
            {
                var status = index switch
                {
                    0 => AttendanceStatus.Present,
                    1 => AttendanceStatus.Present,
                    2 => AttendanceStatus.Late,
                    3 => AttendanceStatus.Present,
                    4 => AttendanceStatus.Absent,
                    5 => AttendanceStatus.Excused,
                    _ => AttendanceStatus.Absent
                };

                DateTime? joinedAt = null;
                DateTime? leftAt = null;
                int? durationMins = null;

                if (status == AttendanceStatus.Present)
                {
                    joinedAt = completedSession.StartTime.AddMinutes(rng.Next(0, 5));
                    leftAt = completedSession.EndTime.AddMinutes(-rng.Next(0, 3));
                    durationMins = (int)(leftAt.Value - joinedAt.Value).TotalMinutes;
                }
                else if (status == AttendanceStatus.Late)
                {
                    joinedAt = completedSession.StartTime.AddMinutes(rng.Next(10, 25));
                    leftAt = completedSession.EndTime;
                    durationMins = (int)(leftAt.Value - joinedAt.Value).TotalMinutes;
                }

                completedSession.Attendances.Add(new SessionAttendance
                {
                    StudentId = student.Id,
                    Status = status,
                    JoinedAt = joinedAt,
                    LeftAt = leftAt,
                    DurationMinutes = durationMins,
                    Note = status == AttendanceStatus.Excused ? "Medical leave" : null
                });
            }

            db.LiveSessions.Add(completedSession);

            // ── Session 2: LIVE (happening right now) ────────────────────────
            var liveSession = new LiveSession
            {
                CourseId = course.Id,
                InstructorId = instructorId,
                Title = $"Week 2 Live Class — {course.Title}",
                Description = "Interactive lecture with exercises.",
                StartTime = now.AddMinutes(-20),   // started 20 min ago
                EndTime = now.AddMinutes(40),    // ends in 40 min
                MeetingUrl = "https://meet.example.com/session-w2-live",
                Status = LiveSessionStatus.Live,
                MaxStudents = 30,
                IsRecorded = true
            };

            // Some students already joined the live session
            var liveStudents = students.Take(8).ToList();
            foreach (var (student, index) in liveStudents.Select((s, i) => (s, i)))
            {
                var status = index switch
                {
                    0 => AttendanceStatus.Present,
                    1 => AttendanceStatus.Present,
                    2 => AttendanceStatus.Present,
                    3 => AttendanceStatus.Late,
                    4 => AttendanceStatus.Registered, // not joined yet
                    5 => AttendanceStatus.Registered,
                    6 => AttendanceStatus.Registered,
                    7 => AttendanceStatus.Registered,
                    _ => AttendanceStatus.Registered
                };

                DateTime? joinedAt = status == AttendanceStatus.Present
                    ? liveSession.StartTime.AddMinutes(rng.Next(0, 5))
                    : status == AttendanceStatus.Late
                        ? liveSession.StartTime.AddMinutes(rng.Next(12, 20))
                        : null;

                liveSession.Attendances.Add(new SessionAttendance
                {
                    StudentId = student.Id,
                    Status = status,
                    JoinedAt = joinedAt,
                    LeftAt = null,  // still in session
                    DurationMinutes = joinedAt.HasValue
                        ? (int)(now - joinedAt.Value).TotalMinutes
                        : null
                });
            }

            db.LiveSessions.Add(liveSession);

            // ── Session 3: SCHEDULED (tomorrow) ─────────────────────────────
            var scheduledSession = new LiveSession
            {
                CourseId = course.Id,
                InstructorId = instructorId,
                Title = $"Week 3 Preview — {course.Title}",
                Description = "Overview of next week topics.",
                StartTime = now.AddDays(1).Date.AddHours(10),
                EndTime = now.AddDays(1).Date.AddHours(11),
                MeetingUrl = "https://meet.example.com/session-w3",
                Status = LiveSessionStatus.Scheduled,
                MaxStudents = 30,
                IsRecorded = false
            };

            // Register students so they appear in attendance list
            foreach (var student in students.Take(8))
            {
                scheduledSession.Attendances.Add(new SessionAttendance
                {
                    StudentId = student.Id,
                    Status = AttendanceStatus.Registered
                });
            }

            db.LiveSessions.Add(scheduledSession);
        }

        await db.SaveChangesAsync();
    }
}