using Microsoft.AspNetCore.Mvc;
using Healthy.Infrastructure.Services;

namespace Healthy.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DevController(DatabaseSeeder seeder) : ControllerBase
{
    private readonly DatabaseSeeder _seeder = seeder;

    /// <summary>
    /// Clear all seeded data and reseed (Development only)
    /// </summary>
    [HttpPost("clear-and-reseed")]
    public async Task<IActionResult> ClearAndReseed()
    {
        if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            return NotFound();
        }

        try
        {
            await _seeder.ClearDataAsync();
            await _seeder.SeedAsync();
            
            return Ok(new { message = "Database cleared and reseeded successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    /// <summary>
    /// Get seeding status (Development only)
    /// </summary>
    [HttpGet("seed-status")]
    public async Task<IActionResult> GetSeedStatus()
    {
        if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
        {
            return NotFound();
        }

        try
        {
            var isSeeded = await _seeder.IsDatabaseSeededAsync();
            return Ok(new { isSeeded, message = isSeeded ? "Database has data" : "Database is empty" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
