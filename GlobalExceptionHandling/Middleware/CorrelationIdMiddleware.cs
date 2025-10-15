namespace GlobalExceptionHandling.Middleware;

/// <summary>
/// Middleware that generates or retrieves a correlation ID for each request.
/// This helps track requests across different services and logs.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeaderName = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Try to get correlation ID from request header
        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();

        // If not present, generate a new one
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Store in HttpContext for later use
        context.Items["CorrelationId"] = correlationId;

        // Add to response headers
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;

        await _next(context);
    }
}
