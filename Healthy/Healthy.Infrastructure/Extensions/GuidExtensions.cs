using System;

namespace Healthy.Infrastructure.Extensions;

public static class GuidExtensions
{
    /// <summary>
    /// Safely converts a GUID string to Guid for comparison
    /// Handles case-insensitive parsing
    /// </summary>
    /// <param name="guidString">GUID string to parse</param>
    /// <returns>Parsed GUID or Guid.Empty if parsing fails</returns>
    public static Guid ToGuid(this string? guidString)
    {
        return Guid.TryParse(guidString, out var guid) ? guid : Guid.Empty;
    }
}
