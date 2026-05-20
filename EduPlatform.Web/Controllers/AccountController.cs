using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Domain.Constants;
using EduPlatform.Infrastructure.Services.Storage;
using EduPlatform.Infrastructure.Identity;
using EduPlatform.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// Handles MVC authentication and account management.
///
/// Responsibilities:
/// - Login / Logout / Register
/// - Profile view and edit (own account)
/// - Password change (own account)
/// - Admin: create users, manage roles, reset passwords, unlock accounts
/// </summary>
public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signIn;
    private readonly UserManager<ApplicationUser> _users;
    private readonly RoleManager<IdentityRole> _roles;
    private readonly IBlobStorageService _blob;

    public AccountController(
        SignInManager<ApplicationUser> signIn,
        UserManager<ApplicationUser> users,
        RoleManager<IdentityRole> roles,
        IBlobStorageService blob)
    {
        _signIn = signIn;
        _users = users;
        _roles = roles;
        _blob  = blob;
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Authentication
    // ──────────────────────────────────────────────────────────────────────────

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginVM());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVM model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
            return View(model);

        var result = await _signIn.PasswordSignInAsync(
            model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
                ? Redirect(returnUrl)
                : RedirectToAction("Index", "Dashboard");

        if (result.IsLockedOut)
            ModelState.AddModelError(string.Empty,
                "Account locked. Try again in a few minutes or contact an administrator.");
        else
            ModelState.AddModelError(string.Empty, "Invalid email or password.");

        return View(model);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Dashboard");

        return View(new RegisterVM());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var allowedRoles = new[] { AppRoles.Student, AppRoles.PendingTeacher, AppRoles.Parent };

        if (!allowedRoles.Contains(model.Role))
        {
            ModelState.AddModelError(nameof(model.Role), "Invalid role selected.");
            return View(model);
        }

        if (!await _roles.RoleExistsAsync(model.Role))
        {
            ModelState.AddModelError(nameof(model.Role),
                $"Role '{model.Role}' does not exist. Run the role seeder first.");
            return View(model);
        }

        if (await _users.FindByEmailAsync(model.Email) is not null)
        {
            ModelState.AddModelError(nameof(model.Email), "Email already registered.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            FullName = model.FullName.Trim(),
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _users.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
        {
            AddIdentityErrors(createResult);
            return View(model);
        }

        var roleResult = await _users.AddToRoleAsync(user, model.Role);
        if (!roleResult.Succeeded)
        {
            await _users.DeleteAsync(user);
            AddIdentityErrors(roleResult);
            return View(model);
        }

        await _signIn.SignInAsync(user, isPersistent: false);
        TempData["Success"] = "Account created successfully.";
        return RedirectToAction("Index", "Dashboard");
    }

    // GET: handles browser back-button navigation to /Account/Logout (405 fix)
    // Any GET to this URL simply signs out and redirects — no CSRF token needed
    // because there is no state-changing form data in a GET.
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> LogoutGet()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // POST: the real logout, triggered by the sidebar form with AntiForgery token.
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [ActionName("Logout")]
    public async Task<IActionResult> LogoutPost()
    {
        await _signIn.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Profile
    // ──────────────────────────────────────────────────────────────────────────

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Profile()
    {
        var user = await _users.GetUserAsync(User);
        if (user is null) return RedirectToAction(nameof(Login));

        var roles = await _users.GetRolesAsync(user);
        return View(new ProfileVM
        {
            FullName    = user.FullName,
            Email       = user.Email ?? string.Empty,
            PhoneNumber = user.PhoneNumber,
            AvatarUrl   = user.AvatarUrl,
            Roles       = roles.OrderBy(RoleSortOrder).ToList()
        });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileVM model)
    {
        var user = await _users.GetUserAsync(User);
        if (user is null) return RedirectToAction(nameof(Login));

        if (!ModelState.IsValid)
        {
            model.Email = user.Email ?? string.Empty;
            model.Roles = (await _users.GetRolesAsync(user)).OrderBy(RoleSortOrder).ToList();
            return View(model);
        }

        user.FullName = model.FullName.Trim();
        user.PhoneNumber = model.PhoneNumber;

        var result = await _users.UpdateAsync(user);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            model.Email = user.Email ?? string.Empty;
            model.Roles = (await _users.GetRolesAsync(user)).OrderBy(RoleSortOrder).ToList();
            return View(model);
        }

        await _signIn.RefreshSignInAsync(user);
        TempData["Success"] = "Profile updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Change Password
    // ──────────────────────────────────────────────────────────────────────────

    [HttpGet]
    [Authorize]
    public IActionResult ChangePassword() => View(new ChangePasswordVM());

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _users.GetUserAsync(User);
        if (user is null) return RedirectToAction(nameof(Login));

        var result = await _users.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return View(model);
        }

        await _signIn.RefreshSignInAsync(user);
        TempData["Success"] = "Password changed successfully.";
        return RedirectToAction(nameof(Profile));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Admin: User Access Management
    // ──────────────────────────────────────────────────────────────────────────

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public IActionResult CreateUser()
    {
        return View(new AdminCreateUserVM
        {
            AvailableRoles = _roles.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(AdminCreateUserVM model)
    {
        model.AvailableRoles = _roles.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToList();

        if (!ModelState.IsValid)
            return View(model);

        if (!await _roles.RoleExistsAsync(model.Role))
        {
            ModelState.AddModelError(nameof(model.Role), "Selected role does not exist.");
            return View(model);
        }

        if (await _users.FindByEmailAsync(model.Email) is not null)
        {
            ModelState.AddModelError(nameof(model.Email), "Email already registered.");
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            FullName = model.FullName.Trim(),
            PhoneNumber = model.PhoneNumber,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _users.CreateAsync(user, model.Password);
        if (!createResult.Succeeded)
        {
            AddIdentityErrors(createResult);
            return View(model);
        }

        var roleResult = await _users.AddToRoleAsync(user, model.Role);
        if (!roleResult.Succeeded)
        {
            await _users.DeleteAsync(user);
            AddIdentityErrors(roleResult);
            return View(model);
        }

        TempData["Success"] = $"User {user.Email} created successfully.";
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.ContentManager)]
    public async Task<IActionResult> Users()
    {
        var users = _users.Users.OrderBy(u => u.Email).ToList();
        var vm = new UserManagementVM();

        foreach (var u in users)
        {
            var roles = await _users.GetRolesAsync(u);
            var isLockedOut = u.LockoutEnd.HasValue && u.LockoutEnd.Value > DateTimeOffset.UtcNow;

            vm.Users.Add(new UserListItemVM
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email ?? string.Empty,
                EmailConfirmed = u.EmailConfirmed,
                IsLockedOut = isLockedOut,
                LockoutEnd = u.LockoutEnd,
                AccessFailedCount = u.AccessFailedCount,
                Roles = roles.OrderBy(RoleSortOrder).ToList()
            });
        }

        return View(vm);
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> EditRoles(string id)
    {
        var user = await _users.FindByIdAsync(id);
        if (user is null) { TempData["Error"] = "User not found."; return RedirectToAction(nameof(Users)); }

        var userRoles = await _users.GetRolesAsync(user);
        var allRoles = _roles.Roles.OrderBy(r => r.Name).Select(r => r.Name!).ToList();

        return View(new EditUserRolesVM
        {
            UserId = user.Id,
            UserDisplayName = $"{user.FullName} ({user.Email})",
            Roles = allRoles.Select(r => new RoleSelectionVM
            {
                RoleName = r,
                IsSelected = userRoles.Contains(r)
            }).ToList()
        });
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditRoles(EditUserRolesVM model)
    {
        var user = await _users.FindByIdAsync(model.UserId);
        if (user is null) { TempData["Error"] = "User not found."; return RedirectToAction(nameof(Users)); }

        var currentRoles = await _users.GetRolesAsync(user);
        var selectedRoles = model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

        var removeResult = await _users.RemoveFromRolesAsync(user, currentRoles);
        if (!removeResult.Succeeded) { AddIdentityErrors(removeResult); return View(model); }

        if (selectedRoles.Any())
        {
            var addResult = await _users.AddToRolesAsync(user, selectedRoles);
            if (!addResult.Succeeded) { AddIdentityErrors(addResult); return View(model); }
        }

        TempData["Success"] = "User roles updated.";
        return RedirectToAction(nameof(Users));
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin + "," + AppRoles.ContentManager)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Unlock(string id)
    {
        var user = await _users.FindByIdAsync(id);
        if (user is null) { TempData["Error"] = "User not found."; return RedirectToAction(nameof(Users)); }

        await _users.SetLockoutEndDateAsync(user, null);
        await _users.ResetAccessFailedCountAsync(user);

        TempData["Success"] = $"Account unlocked for {user.Email}.";
        return RedirectToAction(nameof(Users));
    }

    [HttpGet]
    [Authorize(Roles = AppRoles.Admin)]
    public async Task<IActionResult> ResetPassword(string id)
    {
        var user = await _users.FindByIdAsync(id);
        if (user is null) { TempData["Error"] = "User not found."; return RedirectToAction(nameof(Users)); }

        return View(new ResetPasswordVM
        {
            UserId = user.Id,
            UserDisplayName = $"{user.FullName} ({user.Email})"
        });
    }

    [HttpPost]
    [Authorize(Roles = AppRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
    {
        var user = await _users.FindByIdAsync(model.UserId);
        if (user is null) { TempData["Error"] = "User not found."; return RedirectToAction(nameof(Users)); }

        model.UserDisplayName = $"{user.FullName} ({user.Email})";

        if (!ModelState.IsValid)
            return View(model);

        var token = await _users.GeneratePasswordResetTokenAsync(user);
        var result = await _users.ResetPasswordAsync(user, token, model.NewPassword);

        if (!result.Succeeded)
        {
            AddIdentityErrors(result);
            return View(model);
        }

        // Unlock account if it was locked
        await _users.SetLockoutEndDateAsync(user, null);
        await _users.ResetAccessFailedCountAsync(user);

        TempData["Success"] = $"Password reset for {user.Email}.";
        return RedirectToAction(nameof(Users));
    }

    // ──────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var err in result.Errors)
            ModelState.AddModelError(string.Empty, err.Description);
    }

    /// <summary>
    /// Display priority for role badges.  Lower = shown first.
    /// Admin → ContentManager → Teacher → PendingTeacher → Parent → Student
    /// </summary>
    private static int RoleSortOrder(string role) => role switch
    {
        AppRoles.Admin => 0,
        AppRoles.ContentManager => 1,
        AppRoles.Teacher => 2,
        AppRoles.PendingTeacher => 3,
        AppRoles.Parent => 4,
        AppRoles.Student => 5,
        _ => 99
    };

    // ── Upload Avatar ─────────────────────────────────────────────────────────

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UploadAvatar(IFormFile? avatar, CancellationToken ct)
    {
        var user = await _users.GetUserAsync(User);
        if (user is null) return Forbid();

        var error = FileValidator.ValidateImage(avatar);
        if (error is not null)
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Profile));
        }

        await _blob.DeleteAsync(user.AvatarUrl, ct);

        await using var stream = avatar!.OpenReadStream();
        user.AvatarUrl = await _blob.UploadAsync(
            stream, avatar.FileName, avatar.ContentType, FileValidator.Avatars, ct);

        await _users.UpdateAsync(user);
        TempData["Success"] = "Avatar updated successfully.";
        return RedirectToAction(nameof(Profile));
    }

    // ── Remove Avatar ─────────────────────────────────────────────────────────

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveAvatar(CancellationToken ct)
    {
        var user = await _users.GetUserAsync(User);
        if (user is null) return Forbid();

        await _blob.DeleteAsync(user.AvatarUrl, ct);
        user.AvatarUrl = null;
        await _users.UpdateAsync(user);
        TempData["Success"] = "Avatar removed.";
        return RedirectToAction(nameof(Profile));
    }

}
