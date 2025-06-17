using WPHBookingSystem.Application;
using WPHBookingSystem.Infrastructure;

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

// Add MVC controllers for API endpoints
builder.Services.AddControllers();

// Configure OpenAPI/Swagger documentation
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger UI in development environment
    app.MapOpenApi();
}

// Redirect HTTP requests to HTTPS for security
app.UseHttpsRedirection();

// Enable authorization middleware for protected endpoints
app.UseAuthorization();

// Map API controllers to routes
app.MapControllers();

// Start the application
app.Run();
