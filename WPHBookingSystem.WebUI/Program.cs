using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using WPHBookingSystem.Application;
using WPHBookingSystem.Infrastructure;
//using Microsoft.AspNetCore.Mvc;
/// <summary>
/// Main entry point for the WPH Hotel Booking System Web API.
/// Configures the application, registers services, and sets up the HTTP request pipeline.
/// 
/// This file follows the minimal hosting model introduced in .NET 6+ and configures:
/// - Dependency injection for all layers (Application, Infrastructure)
/// - Controllers and API endpoints
/// - OpenAPI/Swagger documentation
/// - Authentication and authorization middleware
/// - HTTPS redirection and security headers
/// </summary>

var builder = WebApplication.CreateBuilder(args);
//ConfigurationManager configuration = builder.Configuration;

// Register application layer services (use cases, facades, DTOs)
builder.Services.ApplicationDependencyiInjection();

// Register infrastructure layer services (database, identity, repositories)
builder.Services.AddInfrastructureInjection(builder.Configuration);

// Add MVC controllers for API endpoints with proper validation
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
// Configure API behavior for consistent error responses
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = false;
    options.InvalidModelStateResponseFactory = actionContext =>
    {
        var errors = actionContext.ModelState
            .Where(e => e.Value.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        return new BadRequestObjectResult(new
        {
            success = false,
            message = "One or more validation errors occurred.",
            status = 400,
            errors = errors,
            traceId = actionContext.HttpContext.TraceIdentifier
        });
    };
});

// Configure OpenAPI/Swagger documentation
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development environment
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Redirect HTTP requests to HTTPS for security
app.UseHttpsRedirection();

// Enable static file serving for uploaded images
app.UseStaticFiles();

// Enable authentication middleware (must come before authorization)
app.UseAuthentication();

// Enable authorization middleware for protected endpoints
app.UseAuthorization();

// Map API controllers to routes
app.MapControllers();

// Start the application
app.Run();
