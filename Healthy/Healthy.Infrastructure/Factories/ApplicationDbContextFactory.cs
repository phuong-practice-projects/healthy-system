using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Healthy.Infrastructure.Persistence;

namespace Healthy.Infrastructure.Factories
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration to read from appsettings and environment files
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Healthy.Api"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            // Check if we're running in development and database container is available
            var connectionString = GetConnectionString(configuration);

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }

        private string GetConnectionString(IConfiguration configuration)
        {
            // First try to get from environment variables (Docker)
            var dockerConnectionString = configuration.GetConnectionString("DefaultConnection");
            
            if (!string.IsNullOrEmpty(dockerConnectionString))
            {
                // If we're running migrations from host, replace healthy-db with localhost
                if (dockerConnectionString.Contains("healthy-db"))
                {
                    return dockerConnectionString.Replace("healthy-db", "localhost");
                }
                return dockerConnectionString;
            }

            // Fallback to localhost connection for migrations
            return "Server=localhost,1433;Database=HealthyDB_Dev;User Id=sa;Password=Dev@Passw0rd123;TrustServerCertificate=true;";
        }
    }
}
