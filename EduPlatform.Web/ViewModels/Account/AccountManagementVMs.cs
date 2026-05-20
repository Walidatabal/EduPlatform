using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Web.ViewModels.Account;

/// <summary>
/// User profile page ViewModel.
/// Designed for UI editing only; it does not expose Identity internals.
/// </summary>
public class ProfileVM
{
    [Display(Name = "Full Name")]
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(120, ErrorMessage = "Full name cannot exceed 120 characters")]
    public string FullName { get; set; } = string.Empty;

    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Phone Number")]
    [Phone(ErrorMessage = "Enter a valid phone number")]
    public string? PhoneNumber { get; set; }

    public IReadOnlyList<string> Roles { get; set; } = [];
}

/// <summary>
/// Authenticated user password change ViewModel.
/// </summary>
public class ChangePasswordVM
{
    [Required(ErrorMessage = "Current password is required")]
    [DataType(DataType.Password)]
    [Display(Name = "Current Password")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm New Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// Admin reset password ViewModel.
/// Used when an admin needs to reset a user's password without knowing the old password.
/// </summary>
public class ResetPasswordVM
{
    public string UserId { get; set; } = string.Empty;

    [Display(Name = "User")]
    public string UserDisplayName { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm New Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

/// <summary>
/// User list page ViewModel for admin access management.
/// </summary>
public class UserManagementVM
{
    public List<UserListItemVM> Users { get; set; } = [];
}

/// <summary>
/// Single user row used in admin user management.
/// </summary>
public class UserListItemVM
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public bool IsLockedOut { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }
    public int AccessFailedCount { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = [];
}

/// <summary>
/// Admin role edit ViewModel.
/// </summary>
public class EditUserRolesVM
{
    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public List<RoleSelectionVM> Roles { get; set; } = [];
}

/// <summary>
/// Checkbox item for role management.
/// </summary>
public class RoleSelectionVM
{
    public string RoleName { get; set; } = string.Empty;
    public bool IsSelected { get; set; }
}


/// <summary>
/// Admin create user ViewModel.
/// Used by Admin to create users directly from the access control page.
/// </summary>
public class AdminCreateUserVM
{
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(120, ErrorMessage = "Full name cannot exceed 120 characters")]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Enter a valid email address")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Enter a valid phone number")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [Display(Name = "Password")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirm password is required")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Role is required")]
    [Display(Name = "Role")]
    public string Role { get; set; } = string.Empty;

    public List<string> AvailableRoles { get; set; } = [];
}
