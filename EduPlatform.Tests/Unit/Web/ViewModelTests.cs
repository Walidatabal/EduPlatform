using EduPlatform.Web.ViewModels;
using EduPlatform.Web.ViewModels.Account;
using EduPlatform.Web.ViewModels.Enrollments;
using EduPlatform.Web.ViewModels.Orders;

namespace EduPlatform.Tests.Unit.Web;

/// <summary>
/// Unit tests for Web ViewModel default state and computed properties.
///
/// What are ViewModels?
/// ViewModels are the data containers passed from MVC controllers to Razor views.
/// They are purpose-built for UI rendering — they combine data from multiple DTOs,
/// compute display values, and expose only what the view needs.
///
/// Why test ViewModel defaults?
/// ViewModels are instantiated by controllers before they are populated.
/// If a collection property is null (not initialized to an empty list), Razor
/// will throw a NullReferenceException when it calls @foreach on the collection.
/// These tests verify that all collection properties default to empty lists,
/// not null, preventing a whole class of runtime exceptions.
///
/// PendingTasksVM.TotalPending:
/// This computed property is tested with multiple scenarios because it drives
/// the sidebar badge count. An off-by-one error here would display misleading
/// information to admins.
/// </summary>
public class ViewModelTests
{
    // ── PendingTasksVM ───────────────────────────────────────────────────────

    /// <summary>
    /// TotalPending must sum all three categories correctly.
    /// 2 teachers + 1 course + 3 locked users = 6 total.
    /// This is the value displayed in the sidebar badge.
    /// </summary>
    [Fact]
    public void PendingTasksVM_TotalPending_Sums_All_Categories()
    {
        var vm = new PendingTasksVM
        {
            PendingTeachers = new List<PendingTeacherVM> { new(), new() },        // 2
            PendingCourses  = new List<PendingCourseVM>  { new() },               // 1
            LockedUsers     = new List<LockedUserVM>     { new(), new(), new() }  // 3
        };

        Assert.Equal(6, vm.TotalPending);
    }

    /// <summary>
    /// When there are no pending items, TotalPending must be exactly 0.
    /// The sidebar badge should NOT render (or render "0") in this case.
    /// </summary>
    [Fact]
    public void PendingTasksVM_TotalPending_Zero_When_Empty()
    {
        var vm = new PendingTasksVM();
        Assert.Equal(0, vm.TotalPending);
    }

    // ── ProfileVM ────────────────────────────────────────────────────────────

    /// <summary>
    /// ProfileVM.Roles must default to an empty list, not null.
    /// The profile view renders @foreach on Roles — null would throw.
    /// </summary>
    [Fact]
    public void ProfileVM_Roles_DefaultsToEmpty()
    {
        var vm = new ProfileVM();
        Assert.Empty(vm.Roles);
    }

    // ── UserManagementVM ─────────────────────────────────────────────────────

    /// <summary>
    /// The Users list must default to an empty list, not null.
    /// Prevents NullReferenceException when the user table has no records yet.
    /// </summary>
    [Fact]
    public void UserManagementVM_Users_DefaultsToEmptyList()
    {
        var vm = new UserManagementVM();
        Assert.NotNull(vm.Users);
        Assert.Empty(vm.Users);
    }

    // ── EnrollmentIndexVM ────────────────────────────────────────────────────

    /// <summary>
    /// The Enrollments list must default to empty.
    /// A student with no enrollments sees the empty-state message, not an exception.
    /// </summary>
    [Fact]
    public void EnrollmentIndexVM_Enrollments_DefaultsToEmptyList()
    {
        var vm = new EnrollmentIndexVM();
        Assert.NotNull(vm.Enrollments);
        Assert.Empty(vm.Enrollments);
    }

    // ── OrdersIndexVM ────────────────────────────────────────────────────────

    /// <summary>
    /// The Orders list must default to empty.
    /// A new student with no orders sees the empty-state message.
    /// </summary>
    [Fact]
    public void OrdersIndexVM_Orders_DefaultsToEmptyList()
    {
        var vm = new OrdersIndexVM();
        Assert.NotNull(vm.Orders);
        Assert.Empty(vm.Orders);
    }

    // ── CheckoutVM ───────────────────────────────────────────────────────────

    /// <summary>
    /// CheckoutVM.Items must default to empty.
    /// If a student navigates directly to /Orders/Checkout with an empty cart,
    /// the view renders the empty-cart message instead of throwing.
    /// </summary>
    [Fact]
    public void CheckoutVM_Items_DefaultsToEmptyList()
    {
        var vm = new CheckoutVM();
        Assert.NotNull(vm.Items);
        Assert.Empty(vm.Items);
    }

    // ── OrderItemVM ──────────────────────────────────────────────────────────

    /// <summary>
    /// OrderItemVM.Items (the line items within an order) must default to empty.
    /// An order display page must handle 0 items gracefully.
    /// </summary>
    [Fact]
    public void OrderItemVM_Items_DefaultsToEmptyList()
    {
        var vm = new OrderItemVM();
        Assert.NotNull(vm.Items);
        Assert.Empty(vm.Items);
    }
}
