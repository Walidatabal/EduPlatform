using EduPlatform.Domain.Enums;

namespace EduPlatform.Tests.Unit.Domain;

/// <summary>
/// Tests that verify the integer values of all domain enums.
///
/// Why test enum integer values?
/// Enums are stored as integers in SQL Server.
/// If a developer renames an enum member or changes its order,
/// the integer value changes silently. Rows already in the database
/// would then be interpreted as a different enum value, causing
/// data corruption that is extremely difficult to debug.
///
/// Example: if CourseStatus.Draft = 1 is changed to CourseStatus.Draft = 0,
/// all courses currently stored as 1 (Draft) in the database would now
/// be interpreted as 0 (undefined), breaking every course query.
///
/// These tests act as a schema contract:
/// "Draft must ALWAYS be 1 — don't change this without a database migration."
///
/// Every enum value across the entire domain is tested here.
/// </summary>
public class EnumTests
{
    // ── CourseStatus ─────────────────────────────────────────────────────────

    /// <summary>Draft = 1: course is in preparation, not visible to students.</summary>
    [Fact] public void CourseStatus_Draft_Is_1()     => Assert.Equal(1, (int)CourseStatus.Draft);

    /// <summary>Published = 2: teacher declared the course ready for review.</summary>
    [Fact] public void CourseStatus_Published_Is_2() => Assert.Equal(2, (int)CourseStatus.Published);

    /// <summary>Archived = 3: course is retired, no new enrollments allowed.</summary>
    [Fact] public void CourseStatus_Archived_Is_3()  => Assert.Equal(3, (int)CourseStatus.Archived);

    // ── ApprovalStatus ───────────────────────────────────────────────────────

    /// <summary>Pending = 1: waiting for admin review. Default on all new courses.</summary>
    [Fact] public void ApprovalStatus_Pending_Is_1()  => Assert.Equal(1, (int)ApprovalStatus.Pending);

    /// <summary>Approved = 2: admin approved — combined with Published makes course visible.</summary>
    [Fact] public void ApprovalStatus_Approved_Is_2() => Assert.Equal(2, (int)ApprovalStatus.Approved);

    /// <summary>Rejected = 3: admin rejected — teacher must revise and resubmit.</summary>
    [Fact] public void ApprovalStatus_Rejected_Is_3() => Assert.Equal(3, (int)ApprovalStatus.Rejected);

    // ── EnrollmentStatus ─────────────────────────────────────────────────────

    /// <summary>Active = 1: student has access to course content. Default on enrollment.</summary>
    [Fact] public void EnrollmentStatus_Active_Is_1()    => Assert.Equal(1, (int)EnrollmentStatus.Active);

    /// <summary>Completed = 2: all lessons done, certificate eligible.</summary>
    [Fact] public void EnrollmentStatus_Completed_Is_2() => Assert.Equal(2, (int)EnrollmentStatus.Completed);

    // ── OrderStatus ──────────────────────────────────────────────────────────

    /// <summary>Pending = 1: order created, payment not yet confirmed. Default on checkout.</summary>
    [Fact] public void OrderStatus_Pending_Is_1() => Assert.Equal(1, (int)OrderStatus.Pending);

    /// <summary>Paid = 2: payment gateway confirmed payment. Triggers enrollment creation.</summary>
    [Fact] public void OrderStatus_Paid_Is_2()    => Assert.Equal(2, (int)OrderStatus.Paid);

    // ── CertificateStatus ────────────────────────────────────────────────────

    /// <summary>Issued = 1: certificate was generated and is valid.</summary>
    [Fact] public void CertificateStatus_Issued_Is_1()  => Assert.Equal(1, (int)CertificateStatus.Issued);

    /// <summary>Revoked = 2: certificate was cancelled (e.g. student violated terms).</summary>
    [Fact] public void CertificateStatus_Revoked_Is_2() => Assert.Equal(2, (int)CertificateStatus.Revoked);

    // ── PaymentStatus ────────────────────────────────────────────────────────

    /// <summary>Pending = 1: payment request sent to gateway, awaiting response.</summary>
    [Fact] public void PaymentStatus_Pending_Is_1() => Assert.Equal(1, (int)PaymentStatus.Pending);

    /// <summary>Paid = 2: payment gateway confirmed successful charge.</summary>
    [Fact] public void PaymentStatus_Paid_Is_2()    => Assert.Equal(2, (int)PaymentStatus.Paid);

    /// <summary>Failed = 3: payment was declined or timed out.</summary>
    [Fact] public void PaymentStatus_Failed_Is_3()  => Assert.Equal(3, (int)PaymentStatus.Failed);

    // ── QuestionStatus ───────────────────────────────────────────────────────

    /// <summary>Open = 1: question has been posted, awaiting a teacher answer.</summary>
    [Fact] public void QuestionStatus_Open_Is_1()     => Assert.Equal(1, (int)QuestionStatus.Open);

    /// <summary>Answered = 2: a teacher or instructor has provided an answer.</summary>
    [Fact] public void QuestionStatus_Answered_Is_2() => Assert.Equal(2, (int)QuestionStatus.Answered);

    // ── CouponDiscountType ───────────────────────────────────────────────────

    /// <summary>Percentage = 1: discount is a percentage of the subtotal (e.g. 10%).</summary>
    [Fact] public void CouponDiscountType_Percentage_Is_1()  => Assert.Equal(1, (int)CouponDiscountType.Percentage);

    /// <summary>FixedAmount = 2: discount is a fixed KD amount (e.g. 5 KD off).</summary>
    [Fact] public void CouponDiscountType_FixedAmount_Is_2() => Assert.Equal(2, (int)CouponDiscountType.FixedAmount);

    // ── LiveSessionStatus ────────────────────────────────────────────────────

    /// <summary>Scheduled = 1: session is upcoming, link available for students.</summary>
    [Fact] public void LiveSessionStatus_Scheduled_Is_1() => Assert.Equal(1, (int)LiveSessionStatus.Scheduled);

    /// <summary>Live = 2: session is currently active, join link is open.</summary>
    [Fact] public void LiveSessionStatus_Live_Is_2()      => Assert.Equal(2, (int)LiveSessionStatus.Live);

    /// <summary>Completed = 3: session has ended, recording may be available.</summary>
    [Fact] public void LiveSessionStatus_Completed_Is_3() => Assert.Equal(3, (int)LiveSessionStatus.Completed);

    /// <summary>Cancelled = 4: session was cancelled by the teacher or admin.</summary>
    [Fact] public void LiveSessionStatus_Cancelled_Is_4() => Assert.Equal(4, (int)LiveSessionStatus.Cancelled);
}
