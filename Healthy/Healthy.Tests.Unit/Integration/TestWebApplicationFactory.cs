using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Healthy.Api;
using Healthy.Infrastructure.Persistence;
using Healthy.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;

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
            
            // Remove existing authorization services
            var authDescriptors = services.Where(d =>
                d.ServiceType == typeof(IAuthorizationHandler) ||
                d.ServiceType == typeof(IAuthorizationService))
                .ToList();
            foreach (var descriptor in authDescriptors)
            {
                services.Remove(descriptor);
            }

            // Add test authentication
            services.AddAuthentication("Test")
                .AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                    "Test", options => { });

            // Override authorization to allow all requests
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder("Test")
                    .RequireAssertion(_ => true)
                    .Build();
            });

            // Add test authorization handler that allows everything
            services.AddSingleton<IAuthorizationHandler, TestAuthorizationHandler>();
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

// Test authentication scheme options
public class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions { }

// Test authentication handler that always succeeds
public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IOptionsMonitor<TestAuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "TestUser"),
            new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
            new Claim(ClaimTypes.Role, "User"),
            new Claim(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

// Test authorization handler that allows everything
public class TestAuthorizationHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        foreach (var requirement in context.Requirements)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
