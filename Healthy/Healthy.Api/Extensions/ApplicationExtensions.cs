namespace Healthy.Api.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigureApplicationLifetime(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        // Subscribe to application started event to display URLs
        var lifetime = app.ApplicationServices.GetRequiredService<IHostApplicationLifetime>();
        
        lifetime.ApplicationStarted.Register(() =>
        {
            var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();
            var urls = configuration["ASPNETCORE_URLS"]?.Split(';') ?? new[] { "http://localhost:8080" };
            
            Console.WriteLine();
            Console.WriteLine("Application is now running on:");
            foreach (var url in urls)
            {
                // Convert IPv6 wildcard to localhost for display
                var displayUrl = url.Replace("http://[::]:", "http://localhost:")
                                   .Replace("https://[::]:", "https://localhost:")
                                   .Replace("http://+:", "http://localhost:")
                                   .Replace("https://+:", "https://localhost:");
                
                Console.WriteLine($"   > API: {displayUrl}");
                if (environment.IsDevelopment())
                {
                    Console.WriteLine($"   > Swagger: {displayUrl}/swagger");
                }
            }
            Console.WriteLine();
            
            // Display default test accounts in development
            if (environment.IsDevelopment())
            {
                Console.WriteLine("Default Test Accounts:");
                Console.WriteLine("   [ADMIN] admin@healthysystem.com / Admin@123");
                Console.WriteLine("   [USER]  user@healthysystem.com / User@123");
                Console.WriteLine("   [MOD]   moderator@healthysystem.com / Moderator@123");
                Console.WriteLine();
            }
        });
    }

    public static void PrintStartupInfo(IWebHostEnvironment environment)
    {
        // Print host information and Swagger link
        Console.WriteLine();
        Console.WriteLine("Healthy System API");
        Console.WriteLine("==================");
        Console.WriteLine($"Environment: {environment.EnvironmentName}");
        Console.WriteLine($"Started at: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine();

        if (environment.IsDevelopment())
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
    }
}
