using Healthy.Application.Common.Models;
using Healthy.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Healthy.Api.Extensions;

/// <summary>
/// Extension methods for ControllerBase to provide enhanced error handling capabilities
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Creates a standardized error response for validation problems
    /// </summary>
    /// <param name="controller">The controller instance</param>
    /// <param name="errors">Validation error dictionary</param>
    /// <param name="title">Error title</param>
    /// <returns>ValidationProblemDetails response</returns>
    public static ActionResult ValidationError(this ControllerBase controller, Dictionary<string, string[]> errors, string title = "Validation failed")
    {
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Title = title,
            Status = StatusCodes.Status400BadRequest,
            Instance = controller.HttpContext.Request.Path
        };
        
        return controller.BadRequest(problemDetails);
    }
    
    /// <summary>
    /// Creates a standardized error response from ModelState
    /// </summary>
    /// <param name="controller">The controller instance</param>
    /// <param name="title">Error title</param>
    /// <returns>ValidationProblemDetails response</returns>
    public static ActionResult ValidationError(this ControllerBase controller, string title = "Validation failed")
    {
        if (!controller.ModelState.IsValid)
        {
            return controller.ValidationError(controller.ModelState.ToErrorDictionary(), title);
        }
        
        return controller.ValidationError(new Dictionary<string, string[]>(), title);
    }
    
    /// <summary>
    /// Creates a response based on a Result pattern
    /// </summary>
    /// <typeparam name="T">Result value type</typeparam>
    /// <param name="controller">The controller instance</param>
    /// <param name="result">The result object</param>
    /// <returns>Appropriate ActionResult based on result success/failure</returns>
    public static ActionResult<T> HandleResult<T>(this ControllerBase controller, Result<T> result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result);
        }
        
        // Check for specific error patterns to return appropriate status codes
        if (result.Error?.Contains("not found") == true)
        {
            return controller.NotFound(result);
        }
        
        if (result.Error?.Contains("already exists") == true)
        {
            return controller.Conflict(result);
        }
        
        if (result.Error?.Contains("unauthorized") == true || result.Error?.Contains("access denied") == true)
        {
            return controller.Forbid();
        }
        
        return controller.BadRequest(result);
    }
    
    /// <summary>
    /// Creates a response based on a Result pattern (non-generic)
    /// </summary>
    /// <param name="controller">The controller instance</param>
    /// <param name="result">The result object</param>
    /// <returns>Appropriate ActionResult based on result success/failure</returns>
    public static ActionResult HandleResult(this ControllerBase controller, Result result)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result);
        }
        
        // Check for specific error patterns to return appropriate status codes
        if (result.Error?.Contains("not found") == true)
        {
            return controller.NotFound(new { message = result.Error });
        }
        
        if (result.Error?.Contains("already exists") == true)
        {
            return controller.Conflict(new { message = result.Error });
        }
        
        if (result.Error?.Contains("unauthorized") == true || result.Error?.Contains("access denied") == true)
        {
            return controller.Forbid();
        }
        
        return controller.BadRequest(new { message = result.Error });
    }

    /// <summary>
    /// Creates a standardized success response with data
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="controller">The controller instance</param>
    /// <param name="data">Response data</param>
    /// <param name="message">Success message</param>
    /// <returns>Success response</returns>
    public static ActionResult<object> SuccessResponse<T>(this ControllerBase controller, T data, string message = "Operation completed successfully")
    {
        return controller.Ok(new
        {
            success = true,
            message,
            data,
            timestamp = DateTime.UtcNow
        });
    }

    /// <summary>
    /// Creates a standardized error response
    /// </summary>
    /// <param name="controller">The controller instance</param>
    /// <param name="message">Error message</param>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>Error response</returns>
    public static ActionResult ErrorResponse(this ControllerBase controller, string message, int statusCode = StatusCodes.Status400BadRequest)
    {
        var response = new
        {
            success = false,
            message,
            timestamp = DateTime.UtcNow,
            path = controller.HttpContext.Request.Path,
            traceId = controller.HttpContext.TraceIdentifier
        };

        return statusCode switch
        {
            StatusCodes.Status400BadRequest => controller.BadRequest(response),
            StatusCodes.Status401Unauthorized => controller.Unauthorized(response),
            StatusCodes.Status403Forbidden => controller.Forbid(),
            StatusCodes.Status404NotFound => controller.NotFound(response),
            StatusCodes.Status409Conflict => controller.Conflict(response),
            StatusCodes.Status500InternalServerError => controller.StatusCode(StatusCodes.Status500InternalServerError, response),
            _ => controller.StatusCode(statusCode, response)
        };
    }

    /// <summary>
    /// Creates a paginated response
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    /// <param name="controller">The controller instance</param>
    /// <param name="data">Paginated data</param>
    /// <param name="totalCount">Total number of items</param>
    /// <param name="pageNumber">Current page number</param>
    /// <param name="pageSize">Items per page</param>
    /// <returns>Paginated response</returns>
    public static ActionResult<object> PaginatedResponse<T>(this ControllerBase controller, IEnumerable<T> data, int totalCount, int pageNumber, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        return controller.Ok(new
        {
            success = true,
            data,
            pagination = new
            {
                totalCount,
                pageNumber,
                pageSize,
                totalPages,
                hasNextPage = pageNumber < totalPages,
                hasPreviousPage = pageNumber > 1
            },
            timestamp = DateTime.UtcNow
        });
    }
}
