namespace Healthy.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCustomCors(this IServiceCollection services, IWebHostEnvironment environment)
    {
        services.AddCors(options =>
        {
            if (environment.IsDevelopment())
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

        return services;
    }

    public static IApplicationBuilder UseCustomCors(this IApplicationBuilder app, IWebHostEnvironment environment)
    {
        if (environment.IsDevelopment())
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
                    Console.WriteLine($"🚀 CORS Preflight Request:");
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

        return app;
    }
}
