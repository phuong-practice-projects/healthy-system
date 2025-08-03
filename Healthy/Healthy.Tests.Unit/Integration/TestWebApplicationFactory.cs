using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Healthy.Api;
using Healthy.Infrastructure.Persistence;
using Healthy.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace Healthy.Tests.Unit.Integration;

public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
{
    private readonly string _databaseName = $"TestDb_{Guid.NewGuid()}";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove all EF Core related services
            var descriptors = services.Where(d =>
                d.ServiceType == typeof(ApplicationDbContext) ||
                d.ServiceType == typeof(IApplicationDbContext) ||
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                d.ServiceType.IsGenericType && d.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>) ||
                d.ImplementationType?.Assembly.GetName().Name == "Microsoft.EntityFrameworkCore.SqlServer")
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Add InMemory database for testing - use shared database name
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName)
                       .EnableSensitiveDataLogging();
            });

            // Re-add the interface
            services.AddScoped<IApplicationDbContext>(provider => 
                provider.GetRequiredService<ApplicationDbContext>());

            // Add other infrastructure services that test needs
            services.AddScoped<Healthy.Application.Common.Interfaces.IDateTime, Healthy.Infrastructure.Services.DateTimeService>();
            services.AddScoped<Healthy.Application.Common.Interfaces.ICurrentUserService, Healthy.Infrastructure.Services.CurrentUserService>();
            services.AddScoped<Healthy.Application.Common.Interfaces.IJwtService, Healthy.Infrastructure.Services.JwtService>();
        });

        builder.UseEnvironment("Testing");
        
        // Suppress logging during tests
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.SetMinimumLevel(LogLevel.Warning);
        });
    }
    
    public string DatabaseName => _databaseName;
}
