using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Healthy.Infrastructure.Extensions;

public static class ModelStateExtensions
{
    /// <summary>
    /// Converts ModelState errors to a dictionary format
    /// </summary>
    /// <param name="modelState">The ModelState to convert</param>
    /// <returns>Dictionary with field names as keys and error messages array as values</returns>
    public static Dictionary<string, string[]> ToErrorDictionary(this ModelStateDictionary modelState)
    {
        return modelState
            .Where(x => x.Value?.Errors.Count > 0)
            .ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
            );
    }
    
    /// <summary>
    /// Gets all error messages as a single concatenated string
    /// </summary>
    /// <param name="modelState">The ModelState to process</param>
    /// <param name="separator">Separator between error messages</param>
    /// <returns>Concatenated error messages</returns>
    public static string GetErrorMessages(this ModelStateDictionary modelState, string separator = "; ")
    {
        return string.Join(separator, 
            modelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
        );
    }
}
