using DotNetEnv;

namespace Healthy.Api.Extensions;

public static class EnvironmentExtensions
{
    public static void LoadEnvironmentVariables()
    {
        // Load environment variables from .env files
        var envFile = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
            ? ".env.development"
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
        var jwtSecret = Environment.GetEnvironmentVariable("JwtSettings__SecretKey");
        Console.WriteLine($"🔑 JWT_SECRET loaded: {(string.IsNullOrEmpty(jwtSecret) ? "⚠️ NOT FOUND" : "✅ FOUND")}");

        // Set default JWT_SECRET if not found
        if (string.IsNullOrEmpty(jwtSecret))
        {
            Environment.SetEnvironmentVariable("JwtSettings__SecretKey", "dev-super-secret-key-with-at-least-32-characters-for-development");
            Console.WriteLine("🔧 Set default JWT_SECRET");
        }
    }
}
