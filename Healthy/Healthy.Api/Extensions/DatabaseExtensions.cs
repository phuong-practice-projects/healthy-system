using Healthy.Infrastructure.Extensions;

namespace Healthy.Api.Extensions;

public static class DatabaseExtensions
{
    public static async Task<IApplicationBuilder> UseDatabaseAsync(this IApplicationBuilder app)
    {
        // Seed database
        try
        {
            await app.ApplicationServices.SeedDatabaseAsync();
        }
        catch (Exception ex)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred seeding the database");
        }

        return app;
    }
}
