using Healthy.Application;
using Healthy.Infrastructure;
using Healthy.Infrastructure.Extensions;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Healthy.Application.Common.Models;

// Load environment variables from .env files
var envFile = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
    ? "env.development"
    : ".env";

// Try to find the env file in the project root (go up 2 levels from API project)
var projectRoot = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", ".."));
var envFilePath = Path.Combine(projectRoot, envFile);

Console.WriteLine($"🔍 Looking for env file at: {envFilePath}");

if (File.Exists(envFilePath))
{
    Env.Load(envFilePath);
    Console.WriteLine($"✅ Loaded environment file: {envFilePath}");
}
else if (File.Exists(envFile))
{
    // Fallback to current directory
    Env.Load(envFile);
    Console.WriteLine($"✅ Loaded environment file: {envFile}");
}
else
{
    Console.WriteLine($"⚠️ Environment file not found: {envFilePath} or {envFile}");
}

// Debug: Print JWT_SECRET to verify it's loaded
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
Console.WriteLine($"🔑 JWT_SECRET loaded: {(string.IsNullOrEmpty(jwtSecret) ? "NOT FOUND" : "FOUND")}");

// Set default JWT_SECRET if not found
if (string.IsNullOrEmpty(jwtSecret))
{
    Environment.SetEnvironmentVariable("JWT_SECRET", "dev-super-secret-key-with-at-least-32-characters-for-development");
    Console.WriteLine("🔧 Set default JWT_SECRET");
}

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add CORS
builder.Services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment())
    {
        // Development - Allow any origin with credentials (HTTP & HTTPS)
        options.AddPolicy("Development", policy =>
        {
            policy.SetIsOriginAllowed(_ => true) // Allow any origin (HTTP & HTTPS)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials() // Support authentication
                  .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
        });
    }
    else
    {
        // Production - More restrictive
        options.AddPolicy("Production", policy =>
        {
            policy.WithOrigins("https://your-production-domain.com") // Replace with actual domain
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    }

    // Fallback policy for compatibility
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Healthy API", Version = "v1" });

    // Enable annotations
    c.EnableAnnotations();

    // Include XML comments
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add Application Layer
builder.Services.AddApplication();

// Add Infrastructure Layer (skip in test environment to avoid conflicts)
if (builder.Environment.EnvironmentName != "Testing")
{
    builder.Services.AddInfrastructure(builder.Configuration);
}

// Add HttpContextAccessor for CurrentUserService
builder.Services.AddHttpContextAccessor();

// Configure JWT Settings
builder.Services.Configure<JwtSettings>(options =>
{
    var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                    Environment.GetEnvironmentVariable("JwtSettings__SecretKey") ??
                    "dev-super-secret-key-with-at-least-32-characters-for-development";

    // Ensure the key has sufficient length for JWT
    if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 16)
    {
        secretKey = "dev-super-secret-key-with-at-least-32-characters-for-development";
    }

    options.SecretKey = secretKey;
    options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                     Environment.GetEnvironmentVariable("JwtSettings__Issuer") ??
                     "HealthySystem";
    options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                      Environment.GetEnvironmentVariable("JwtSettings__Audience") ??
                      "HealthySystemUsers";
    options.ExpirationInMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRY_HOURS"), out var hours) ? hours * 60 : 60;
    options.RefreshTokenExpirationInDays = 7;
});

// Add JWT Authentication
var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                Environment.GetEnvironmentVariable("JwtSettings__SecretKey") ??
                "dev-super-secret-key-with-at-least-32-characters-for-development";

// Ensure the key has sufficient length for JWT
if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 16)
{
    secretKey = "dev-super-secret-key-with-at-least-32-characters-for-development";
}

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment(); // Only require HTTPS in production
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                     Environment.GetEnvironmentVariable("JwtSettings__Issuer") ??
                     "HealthySystem",
        ValidateAudience = true,
        ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                      Environment.GetEnvironmentVariable("JwtSettings__Audience") ??
                      "HealthySystemUsers",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Seed database
try
{
    await app.Services.SeedDatabaseAsync();
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the database");
}

// Subscribe to application started event to display URLs
app.Lifetime.ApplicationStarted.Register(() =>
{
    var urls = app.Urls;
    Console.WriteLine();
    Console.WriteLine("Application is now running on:");
    foreach (var url in urls)
    {
        // Convert IPv6 wildcard to localhost for display
        var displayUrl = url.Replace("http://[::]:", "http://localhost:")
                           .Replace("https://[::]:", "https://localhost:");
        
        Console.WriteLine($"   > API: {displayUrl}");
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine($"   > Swagger: {displayUrl}/swagger");
        }
    }
    Console.WriteLine();
    
    // Display default test accounts in development
    if (app.Environment.IsDevelopment())
    {
        Console.WriteLine("Default Test Accounts:");
        Console.WriteLine("   [ADMIN] admin@healthysystem.com / Admin@123");
        Console.WriteLine("   [USER]  user@healthysystem.com / User@123");
        Console.WriteLine("   [MOD]   moderator@healthysystem.com / Moderator@123");
        Console.WriteLine();
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Healthy API V1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at root
    });
}

// Only use HTTPS redirection in production
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Add CORS middleware FIRST - Use environment-specific policy
if (app.Environment.IsDevelopment())
{
    // Add detailed CORS debugging middleware
    app.Use(async (context, next) =>
    {
        var origin = context.Request.Headers["Origin"].FirstOrDefault();
        var method = context.Request.Method;
        var path = context.Request.Path;
        var scheme = context.Request.Scheme;
        
        Console.WriteLine($"🔍 Request: {method} {scheme}://{context.Request.Host}{path}");
        Console.WriteLine($"   Origin: {origin ?? "none"}");
        
        if (method == "OPTIONS")
        {
            Console.WriteLine($"� CORS Preflight Request:");
            Console.WriteLine($"   Access-Control-Request-Method: {context.Request.Headers["Access-Control-Request-Method"]}");
            Console.WriteLine($"   Access-Control-Request-Headers: {context.Request.Headers["Access-Control-Request-Headers"]}");
        }
        
        await next();
        
        Console.WriteLine($"📤 Response Status: {context.Response.StatusCode}");
        var corsHeaders = context.Response.Headers.Where(h => h.Key.StartsWith("Access-Control"));
        if (corsHeaders.Any())
        {
            Console.WriteLine("   CORS Headers:");
            foreach (var header in corsHeaders)
            {
                Console.WriteLine($"     {header.Key}: {header.Value}");
            }
        }
        Console.WriteLine();
    });
    
    app.UseCors("Development");
    Console.WriteLine("[CORS] Development policy applied (HTTP & HTTPS allowed)");
}
else
{
    app.UseCors("Production");
    Console.WriteLine("[CORS] Production policy applied");
}

// Global exception handling (should be early in pipeline)
app.UseGlobalExceptionHandler();

// Built-in Authentication & Authorization (still needed for some features)
app.UseAuthentication();
app.UseAuthorization();
// Add custom User Authorization middleware FIRST (extracts JWT and populates user context)
app.UseCustomMiddlewares();
app.MapControllers();

// Print host information and Swagger link
Console.WriteLine();
Console.WriteLine("Healthy System API");
Console.WriteLine("==================");
Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"Started at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
Console.WriteLine();

if (app.Environment.IsDevelopment())
{
    Console.WriteLine("[OK] Development Environment Ready!");
    Console.WriteLine("Quick Access:");
    Console.WriteLine($"   > Main API: Available at URLs shown above");
    Console.WriteLine($"   > Swagger UI: Available at URLs shown above + /swagger");
    Console.WriteLine($"   > API Docs: Interactive documentation available");
    Console.WriteLine();
    Console.WriteLine("[READY] Ready to serve requests! Press Ctrl+C to stop.");
    Console.WriteLine("=========================================");
}
else
{
    Console.WriteLine("[OK] Production Environment Ready!");
    Console.WriteLine("Service Status:");
    Console.WriteLine($"   > API: Running (URLs displayed above)");
    Console.WriteLine($"   > Security: Production settings active");
    Console.WriteLine();
    Console.WriteLine("[ONLINE] Service is online! Press Ctrl+C to stop.");
    Console.WriteLine("=========================================");
}

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
