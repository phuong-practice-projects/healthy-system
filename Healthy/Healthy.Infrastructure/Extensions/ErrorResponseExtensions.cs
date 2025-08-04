using Healthy.Infrastructure.Middleware;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Healthy.Infrastructure.Extensions;

public static class ErrorResponseExtensions
{
    /// <summary>
    /// Creates a standardized error response
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <param name="message">Error message</param>
    /// <param name="details">Additional error details (only shown in development)</param>
    /// <param name="validationErrors">Validation errors dictionary</param>
    /// <returns>Formatted error response</returns>
    public static ErrorResponse CreateErrorResponse(
        this HttpContext context,
        HttpStatusCode statusCode,
        string message,
        string? details = null,
        Dictionary<string, string[]>? validationErrors = null)
    {
        var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
        
        return new ErrorResponse
        {
            Message = message,
            StatusCode = (int)statusCode,
            Path = context.Request.Path,
            Method = context.Request.Method,
            Timestamp = DateTime.UtcNow,
            TraceId = context.TraceIdentifier,
            Details = isDevelopment ? details : null,
            ValidationErrors = validationErrors
        };
    }

    /// <summary>
    /// Creates an error response for validation failures
    /// </summary>
    /// <param name="context">HTTP context</param>
    /// <param name="validationErrors">Validation errors dictionary</param>
    /// <param name="message">Custom message (optional)</param>
    /// <returns>Validation error response</returns>
    public static ErrorResponse CreateValidationErrorResponse(
        this HttpContext context,
        Dictionary<string, string[]> validationErrors,
        string message = "Validation failed")
    {
        return context.CreateErrorResponse(
            HttpStatusCode.BadRequest,
            message,
            validationErrors: validationErrors
        );
    }
}
