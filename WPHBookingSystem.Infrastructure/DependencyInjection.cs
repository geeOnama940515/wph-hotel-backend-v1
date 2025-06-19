using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using WPHBookingSystem.Application.DTOs.Email;
using WPHBookingSystem.Application.Interfaces;
using WPHBookingSystem.Application.Interfaces.Services;
using WPHBookingSystem.Infrastructure.Identity;
using WPHBookingSystem.Infrastructure.Persistence.Data;
using WPHBookingSystem.Infrastructure.Repositories;
using WPHBookingSystem.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace WPHBookingSystem.Infrastructure
{
    /// <summary>
    /// Static class responsible for configuring and registering all infrastructure layer services
    /// with the dependency injection container. This class follows the extension method pattern
    /// to provide a clean and organized way to register infrastructure services.
    /// 
    /// The infrastructure layer handles external concerns including:
    /// - Database persistence with Entity Framework Core
    /// - User authentication and authorization with ASP.NET Core Identity
    /// - Repository implementations for data access
    /// - JWT token generation and validation
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers all infrastructure layer services with the dependency injection container.
        /// This extension method should be called during application startup to configure
        /// database, identity, and repository services.
        /// </summary>
        /// <param name="services">The service collection to register services with.</param>
        /// <param name="configuration">The configuration containing connection strings and settings.</param>
        /// <returns>The service collection for method chaining.</returns>
        public static IServiceCollection AddInfrastructureInjection(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Entity Framework Core DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
            });

            // Register ASP.NET Core Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            // Configure JWT Authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JwtSettings:Audience"],
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"])),
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                
                // Add events for better debugging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogWarning("JWT Authentication failed: {Error}", context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("JWT Challenge issued for path: {Path}", context.HttpContext.Request.Path);
                        return Task.CompletedTask;
                    }
                };
            });

            // Configure Identity options for security and validation
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                //options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

            });

            // Register Repositories
            //services.AddScoped<IRoomRepository, RoomRepository>();
            //services.AddScoped<IBookingRepository, BookingRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Services
            services.AddScoped<IIdentityService, IdentityService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IImageUploadService, ImageUploadService>();
            services.AddScoped<IEmailService, EmailService>();

            // Configure Email Settings
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            //services.AddHealthChecks()
            //    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy())
            //    .UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            return services;
        }
    }
}
