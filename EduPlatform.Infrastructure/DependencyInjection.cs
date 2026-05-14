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
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"),
                sql => sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.SignIn.RequireConfirmedEmail = false; // set true in production after email verification is implemented
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Auth / infrastructure services
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IEmailService, EmailService>();

        // LMS services
        services.AddScoped<ILmsPlatformService, LmsPlatformService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<IWishlistService, WishlistService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<ICourseReviewService, CourseReviewService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IProgressService, ProgressService>();
        services.AddScoped<ICertificateService, CertificateService>();
        services.AddScoped<ILiveSessionService, LiveSessionService>();
        services.AddScoped<ICourseService, CourseService>();

        services.AddHttpContextAccessor();

        return services;
    }
}
