using Microsoft.AspNetCore.Mvc;

namespace EduPlatform.Web.Controllers;

/// <summary>
/// Handles simple UI language switching.
///
/// This is a lightweight language switch.
/// Later we can replace it with ASP.NET Core Localization and resource files.
/// </summary>
public class LanguageController : Controller
{
    /// <summary>
    /// Saves selected language in a browser cookie.
    /// </summary>
    public IActionResult Set(string culture)
    {
        // Allow only supported languages.
        if (culture != "ar" && culture != "en")
        {
            culture = "en";
        }

        Response.Cookies.Append(
            "lang",
            culture,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true
            });

        // Return user to previous page.
        var returnUrl = Request.Headers.Referer.ToString();

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = "/";
        }

        return Redirect(returnUrl);
    }
}