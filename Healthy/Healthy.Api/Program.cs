using Healthy.Application;
using Healthy.Infrastructure;
using Healthy.Infrastructure.Extensions;
using Healthy.Api.Extensions;

// Load environment variables
EnvironmentExtensions.LoadEnvironmentVariables();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add custom CORS
builder.Services.AddCustomCors(builder.Environment);

// Add custom Swagger/OpenAPI
builder.Services.AddCustomSwagger();

// Add Application Layer
builder.Services.AddApplication();

// Add Infrastructure Layer (skip in test environment to avoid conflicts)
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.Services.AddInfrastructure(builder.Configuration);
}

// Add HttpContextAccessor for CurrentUserService
builder.Services.AddHttpContextAccessor();

// Add custom Authentication and Authorization (skip in test environment)
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.Services.AddCustomAuthentication();
}

var app = builder.Build();

// Seed database
await app.UseDatabaseAsync();

// Configure application lifetime events
app.ConfigureApplicationLifetime(app.Environment);

// Configure the HTTP request pipeline
app.UseCustomSwagger(app.Environment);

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Add CORS middleware
app.UseCustomCors(app.Environment);

// Global exception handling (should be early in pipeline)
app.UseGlobalExceptionHandler();

// Built-in Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Add custom User Authorization middleware
app.UseCustomMiddlewares();

app.MapControllers();

// Print startup information
ApplicationExtensions.PrintStartupInfo(app.Environment);

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
