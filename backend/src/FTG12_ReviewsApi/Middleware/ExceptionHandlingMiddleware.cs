using System.Text.Json;
using FTG12_ReviewsApi.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using ValidationException = FTG12_ReviewsApi.Application.Common.Exceptions.ValidationException;

namespace FTG12_ReviewsApi.Middleware;

/// <summary>
/// Global exception handling middleware that maps exceptions to RFC 9457 Problem Details responses.
/// </summary>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                    Title = "One or more validation errors occurred.",
                    Status = StatusCodes.Status400BadRequest,
                    Detail = validationEx.Message,
                    Extensions = { ["errors"] = validationEx.Errors }
                }),

            NotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5",
                    Title = "Resource not found.",
                    Status = StatusCodes.Status404NotFound,
                    Detail = notFoundEx.Message
                }),

            ForbiddenException forbiddenEx => (
                StatusCodes.Status403Forbidden,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.4",
                    Title = "Forbidden.",
                    Status = StatusCodes.Status403Forbidden,
                    Detail = forbiddenEx.Message
                }),

            ConflictException conflictEx => (
                StatusCodes.Status409Conflict,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.10",
                    Title = "Conflict.",
                    Status = StatusCodes.Status409Conflict,
                    Detail = conflictEx.Message
                }),

            UnauthorizedAccessException unauthorizedEx => (
                StatusCodes.Status401Unauthorized,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.5.2",
                    Title = "Unauthorized.",
                    Status = StatusCodes.Status401Unauthorized,
                    Detail = unauthorizedEx.Message
                }),

            _ => (
                StatusCodes.Status500InternalServerError,
                new ProblemDetails
                {
                    Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1",
                    Title = "An unexpected error occurred.",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = "An unexpected error occurred."
                })
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "An unhandled exception occurred");
        }
        else
        {
            logger.LogWarning(exception, "A handled exception occurred: {Message}", exception.Message);
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));
    }
}
