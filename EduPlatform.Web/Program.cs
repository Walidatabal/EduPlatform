using EduPlatform.Infrastructure;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

// ── Services ──────────────────────────────────────────────────────────────────
builder.Services.AddControllersWithViews();

// Reuse the exact same Infrastructure layer as the API:
// DbContext, Identity cookie auth, Repositories, UnitOfWork, all LMS services.
builder.Services.AddInfrastructure(builder.Configuration);

// Cookie security policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
    options.HttpOnly              = HttpOnlyPolicy.Always;
    // Only enforce Secure cookies in production.
    // In Development, HTTP is used so Secure=Always would break cookie auth.
    options.Secure = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
});

// ── App pipeline ──────────────────────────────────────────────────────────────
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    // Only redirect HTTP→HTTPS in non-Development environments.
    // In Development, HTTPS redirect causes issues when running without
    // a trusted dev certificate (common on fresh machines or Docker).
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// NOTE: Database seeding and migrations are owned by EduPlatform.API.
// The Web project is a UI consumer only — it never seeds or migrates.

app.Run();
