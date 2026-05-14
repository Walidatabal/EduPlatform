using EduPlatform.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// =========================
// MVC Presentation Layer
// =========================
builder.Services.AddControllersWithViews();

// Reuse the same Infrastructure layer used by the API:
// DbContext, Identity, Repositories, UnitOfWork, CurrentUserService, etc.
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// NOTE: Database migrations and seeding are owned exclusively by EduPlatform.API.
// The Web project is a UI consumer only — it never seeds or migrates the database.

app.Run();
