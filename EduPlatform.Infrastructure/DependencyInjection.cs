using EduPlatform.Application.Common.Interfaces;
using EduPlatform.Application.Features.Lms.Interfaces;
using EduPlatform.Domain.Interfaces;
using EduPlatform.Infrastructure.Data;
using EduPlatform.Infrastructure.Identity;
using EduPlatform.Infrastructure.Repositories;
using EduPlatform.Infrastructure.Services.Auth;
using EduPlatform.Infrastructure.Services.Lms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EduPlatform.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // ── Database ──────────────────────────────────────────────────────────
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                config.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        // ── ASP.NET Core Identity ─────────────────────────────────────────────
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password policy
            options.Password.RequireDigit           = true;
            options.Password.RequireLowercase        = true;
            options.Password.RequireUppercase        = true;
            options.Password.RequireNonAlphanumeric  = false;
            options.Password.RequiredLength          = 8;

            // Lockout policy — 5 failed attempts = 15 min lockout
            options.Lockout.DefaultLockoutTimeSpan   = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts  = 5;
            options.Lockout.AllowedForNewUsers        = true;

            // Email confirmation not required in dev; set true in production
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // Cookie settings for MVC auth
        services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath        = "/Account/Login";
            options.LogoutPath       = "/Account/Logout";
            options.AccessDeniedPath = "/Account/Login";
            options.SlidingExpiration = true;
            options.ExpireTimeSpan   = TimeSpan.FromHours(8);
            options.Cookie.HttpOnly  = true;
            options.Cookie.SameSite  = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
        });

        // ── Repositories / Unit of Work ───────────────────────────────────────
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // ── Auth / infrastructure services ───────────────────────────────────
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();

        // ── LMS feature services ─────────────────────────────────────────────
        services.AddScoped<ILmsPlatformService, LmsPlatformService>();
        services.AddScoped<ICategoryService,    CategoryService>();
        services.AddScoped<ICartService,        CartService>();
        services.AddScoped<IWishlistService,    WishlistService>();
        services.AddScoped<ICouponService,      CouponService>();
        services.AddScoped<ICourseReviewService, CourseReviewService>();
        services.AddScoped<IOrderService,       OrderService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IProgressService,    ProgressService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<ILiveSessionService, LiveSessionService>();
        services.AddScoped<ICourseService,      CourseService>();

        // ── HTTP Context (for CurrentUserService) ─────────────────────────────
        services.AddHttpContextAccessor();

        return services;
    }
}
