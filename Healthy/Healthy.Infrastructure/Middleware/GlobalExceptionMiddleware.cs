using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;
using FluentValidation;
using Healthy.Domain.Exceptions;
using Healthy.Infrastructure.Extensions;

namespace Healthy.Infrastructure.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred. RequestPath: {RequestPath}, Method: {Method}, Message: {Message}", 
                context.Request.Path, context.Request.Method, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse
        {
            Path = context.Request.Path,
            Method = context.Request.Method,
            Timestamp = DateTime.UtcNow,
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ValidationException validationEx:
                errorResponse.Message = "Validation failed";
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.ValidationErrors = validationEx.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(x => x.ErrorMessage).ToArray()
                    );
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case ApplicationException appEx when appEx.Message.Contains("not found"):
                errorResponse.Message = appEx.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case ApplicationException appEx when appEx.Message.Contains("already exists"):
                errorResponse.Message = appEx.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                break;

            case ApplicationException appEx when appEx.Message.Contains("forbidden") || appEx.Message.Contains("access denied"):
                errorResponse.Message = appEx.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                break;

            case ApplicationException appEx:
                errorResponse.Message = appEx.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case KeyNotFoundException:
                errorResponse.Message = "Resource not found";
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case UnauthorizedAccessException:
                errorResponse.Message = "Unauthorized access";
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case ArgumentException argEx:
                errorResponse.Message = argEx.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case InvalidOperationException invalidOpEx:
                errorResponse.Message = invalidOpEx.Message;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case TimeoutException:
                errorResponse.Message = "Request timeout occurred";
                errorResponse.StatusCode = (int)HttpStatusCode.RequestTimeout;
                context.Response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                break;

            default:
                errorResponse.Message = "An internal server error occurred";
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                
                // Only expose detailed error information in development
                var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                if (isDevelopment)
                {
                    errorResponse.Details = exception.Message;
                    errorResponse.StackTrace = exception.StackTrace;
                }
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        await context.Response.WriteAsync(jsonResponse);
    }
}

public class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? TraceId { get; set; }
    public string? Details { get; set; }
    public string? StackTrace { get; set; }
    public Dictionary<string, string[]>? ValidationErrors { get; set; }
}
