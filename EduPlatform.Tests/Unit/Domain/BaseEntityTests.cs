using EduPlatform.Domain.Entities;
using Xunit;

namespace EduPlatform.Tests.Unit.Domain;

public class BaseEntityTests
{
    [Fact]
    public void Grade_New_IsNotDeleted_By_Default()
    {
        var grade = new Grade { Name = "Grade 10" };
        Assert.False(grade.IsDeleted);
    }

    [Fact]
    public void Course_Price_CanBeZero_ForFreeCourse()
    {
        var course = new Course { Title = "Free Course", Price = 0 };
        Assert.Equal(0, course.Price);
    }

    [Fact]
    public void Enrollment_DefaultStatus_IsActive()
    {
        var enrollment = new Enrollment();
        Assert.Equal(EduPlatform.Domain.Enums.EnrollmentStatus.Active, enrollment.Status);
    }

    [Fact]
    public void Course_DefaultApprovalStatus_IsPending()
    {
        var course = new Course { Title = "Test" };
        Assert.Equal(EduPlatform.Domain.Enums.ApprovalStatus.Pending, course.ApprovalStatus);
    }

    [Fact]
    public void Course_DefaultStatus_IsDraft()
    {
        var course = new Course { Title = "Test" };
        Assert.Equal(EduPlatform.Domain.Enums.CourseStatus.Draft, course.Status);
    }
}
