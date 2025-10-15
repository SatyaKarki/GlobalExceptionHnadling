using GlobalExceptionHandling.ProblemDetails;
using System.Text.Json;

namespace GlobalExceptionHandling.Middleware;

/// <summary>
/// Middleware that catches all unhandled exceptions and converts them to appropriate HTTP responses.
/// Uses ProblemDetails format (RFC 7807) for consistent error responses.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IExceptionToProblemDetailsConverter _converter;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IExceptionToProblemDetailsConverter converter,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _converter = converter;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Get correlation ID for tracking
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";

        // Log the exception with correlation ID
        _logger.LogError(
            exception,
            "An unhandled exception occurred. CorrelationId: {CorrelationId}",
            correlationId);

        // Convert exception to ProblemDetails
        var problemDetails = _converter.Convert(exception, context);

        // In development, include stack trace for easier debugging
        if (_environment.IsDevelopment())
        {
            problemDetails.Extensions["exception"] = exception.GetType().Name;
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            problemDetails.Extensions["innerException"] = exception.InnerException?.Message;
        }

        // Set response
        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        await context.Response.WriteAsJsonAsync(problemDetails, options);
    }
}
