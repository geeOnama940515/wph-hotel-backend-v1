using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Http.Features;
using Scalar.AspNetCore;
using WPHBookingSystem.Application;
using WPHBookingSystem.Infrastructure;
using WPHBookingSystem.Infrastructure.Identity;
using WPHBookingSystem.WebUI;
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

// App + Infrastructure layers
builder.Services.ApplicationDependencyiInjection();
builder.Services.AddInfrastructureInjection(builder.Configuration);
builder.Services.WebUIServices();

// Controllers + Routing
builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);

// Swagger (Swashbuckle)
builder.Services.AddEndpointsApiExplorer();

// Request limits (upload)
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 52428800;
});
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 52428800;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 52428800;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});

// API Behavior
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

var app = builder.Build();

// === HTTP Pipeline ===

// Enable Swagger for all environments
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "WPH Hotel Booking API v1");
    c.RoutePrefix = "swagger";
});

// Redirect HTTP → HTTPS (optional)
app.UseHttpsRedirection();

// Serve images, static files
app.UseStaticFiles();

// Log requests (for debugging)
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("Request: {Method} {Path} {ContentType}",
        context.Request.Method,
        context.Request.Path,
        context.Request.ContentType);
    await next();
    logger.LogInformation("Response: {StatusCode}", context.Response.StatusCode);
});

// Auth & CORS
app.UseCors("AllowProdOrigin");
app.UseAuthentication();
app.UseAuthorization();

// Map routes
app.MapControllers();

// Run app
app.Run();
