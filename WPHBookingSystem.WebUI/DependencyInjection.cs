using Microsoft.OpenApi.Models;

namespace WPHBookingSystem.WebUI
{
    /// <summary>
    /// Web UI layer dependency injection configuration.
    /// 
    /// This class provides extension methods for registering Web UI specific services.
    /// Currently minimal as most services are registered in the Application and Infrastructure layers.
    /// Can be extended to include Web UI specific services like:
    /// - Custom middleware
    /// - API versioning
    /// - CORS policies
    /// - Custom authentication schemes
    /// </summary>
    public static class DependencyInjection
    {
        /// <summary>
        /// Registers Web UI specific services with the dependency injection container.
        /// </summary>
        /// <param name="services">The service collection to register services with</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection WebUIServices(this IServiceCollection services)
        {
            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowDevOrigin", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .WithExposedHeaders("Content-Disposition", "Content-Length", "Content-Type")
                           .SetIsOriginAllowed(origin => true); // Allow any origin for development
                });

                options.AddPolicy("AllowProdOrigin", builder =>
                {
                    builder.WithOrigins("https://wph-hotel.gregdoesdev.xyz") // Replace later with actual production URL
                           .AllowAnyMethod()
                           .AllowAnyHeader()
                           .WithExposedHeaders("Content-Disposition", "Content-Length", "Content-Type");
                });
            });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "WPH - HOTEL BACKEND",
                    Version = "v1"
                });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });
            // Web UI specific services can be registered here
            // Currently empty as most services are handled by Application and Infrastructure layers
            return services;
        }
    }
}
