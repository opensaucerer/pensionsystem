using System.Net;
using System.Text.Json;
using FluentValidation;
using PensionSystem.Domain.Exceptions;

namespace PensionSystem.API.Middleware;

/// <summary>
/// Global exception handling middleware that maps exceptions to
/// structured JSON error responses with appropriate HTTP status codes.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception caught by middleware");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        object response;

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                var errors = validationException.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });
                response = new { error = "Validation failed.", statusCode = context.Response.StatusCode, errors };
                break;

            case DomainException domainException:
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                response = new { error = domainException.Message, statusCode = context.Response.StatusCode };
                break;

            default:
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response = new { error = "An unexpected error occurred.", statusCode = context.Response.StatusCode };
                break;
        }

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        await context.Response.WriteAsync(json);
    }
}
