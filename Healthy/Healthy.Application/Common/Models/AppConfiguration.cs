namespace Healthy.Application.Common.Models;

public class AppConfiguration
{
    // Application Settings
    public string Environment { get; set; } = "Development";
    public string Urls { get; set; } = "http://+:8080";
    
    // Database Settings
    public DatabaseConfig Database { get; set; } = new();
    
    // Redis Settings
    public RedisConfig Redis { get; set; } = new();
    
    // API Settings
    public ApiConfig Api { get; set; } = new();
    
    // Logging Settings
    public LoggingConfig Logging { get; set; } = new();
    
    // Security Settings
    public SecurityConfig Security { get; set; } = new();
    
    // CORS Settings
    public CorsConfig Cors { get; set; } = new();
    
    // Rate Limiting Settings
    public RateLimitConfig RateLimiting { get; set; } = new();
    
    // Development Features
    public DevelopmentConfig Development { get; set; } = new();
}

public class DatabaseConfig
{
    public string Server { get; set; } = "localhost";
    public string Name { get; set; } = "HealthyDB";
    public string User { get; set; } = "sa";
    public string Password { get; set; } = "";
    public bool TrustServerCertificate { get; set; } = true;
    
    public string GetConnectionString()
    {
        return $"Server={Server};Database={Name};User Id={User};Password={Password};TrustServerCertificate={TrustServerCertificate};";
    }
}

public class RedisConfig
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 6379;
    
    public string GetConnectionString()
    {
        return $"{Host}:{Port}";
    }
}

public class ApiConfig
{
    public int Port { get; set; } = 5001;
    public string Host { get; set; } = "0.0.0.0";
}

public class LoggingConfig
{
    public string Level { get; set; } = "Information";
    public string MicrosoftLevel { get; set; } = "Warning";
}

public class SecurityConfig
{
    public string JwtSecret { get; set; } = "your-super-secret-jwt-key-here-change-in-production";
    public string JwtIssuer { get; set; } = "healthy-system";
    public string JwtAudience { get; set; } = "healthy-system-api";
    public int JwtExpiryHours { get; set; } = 24;
}

public class CorsConfig
{
    public string[] AllowedOrigins { get; set; } = { "http://localhost:3000", "http://localhost:4200" };
}

public class RateLimitConfig
{
    public int PerMinute { get; set; } = 100;
    public int PerHour { get; set; } = 1000;
}

public class DevelopmentConfig
{
    public bool EnableSwagger { get; set; } = true;
    public bool EnableDetailedErrors { get; set; } = true;
    public bool EnableDeveloperExceptionPage { get; set; } = true;
    public bool UsePollingFileWatcher { get; set; } = true;
    public bool RunningInContainer { get; set; } = true;
} 