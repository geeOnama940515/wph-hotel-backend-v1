using WPHBookingSystem.Application;
using WPHBookingSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
//ConfigurationManager configuration = builder.Configuration;
// Add services to the container.
builder.Services.ApplicationDependencyiInjection();
builder.Services.AddInfrastructureInjection(builder.Configuration);
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
