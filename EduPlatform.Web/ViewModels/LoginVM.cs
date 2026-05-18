// Kept for backward compatibility — new ViewModel is ViewModels/Account/LoginVM.cs
using System.ComponentModel.DataAnnotations;
namespace EduPlatform.Web.ViewModels;
public class LoginVM
{
    [Required][EmailAddress] public string Email    { get; set; } = string.Empty;
    [Required][DataType(DataType.Password)] public string Password { get; set; } = string.Empty;
    public bool RememberMe { get; set; }
}
