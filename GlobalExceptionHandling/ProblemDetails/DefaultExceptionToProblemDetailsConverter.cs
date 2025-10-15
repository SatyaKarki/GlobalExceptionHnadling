using GlobalExceptionHandling.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionHandling.ProblemDetails;

/// <summary>
/// Default implementation of IExceptionToProblemDetailsConverter.
/// Converts various exception types to appropriate ProblemDetails responses.
/// </summary>
public class DefaultExceptionToProblemDetailsConverter : IExceptionToProblemDetailsConverter
{
    public Microsoft.AspNetCore.Mvc.ProblemDetails Convert(Exception exception, HttpContext context)
    {
        var problemDetails = new Microsoft.AspNetCore.Mvc.ProblemDetails
        {
            Instance = context.Request.Path
        };

        // Add correlation ID if available
        if (context.Items.TryGetValue("CorrelationId", out var correlationId))
        {
            problemDetails.Extensions["correlationId"] = correlationId;
        }

        switch (exception)
        {
            case ValidationFailureException validationException:
                problemDetails.Status = validationException.StatusCode;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1";
                problemDetails.Title = "Validation Error";
                problemDetails.Detail = validationException.Message;
                problemDetails.Extensions["errors"] = validationException.Errors;
                break;

            case NotFoundException notFoundException:
                problemDetails.Status = notFoundException.StatusCode;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.5.5";
                problemDetails.Title = "Resource Not Found";
                problemDetails.Detail = notFoundException.Message;
                break;

            case AppException appException:
                problemDetails.Status = appException.StatusCode;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
                problemDetails.Title = "Application Error";
                problemDetails.Detail = appException.Message;
                break;

            default:
                problemDetails.Status = StatusCodes.Status500InternalServerError;
                problemDetails.Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1";
                problemDetails.Title = "Internal Server Error";
                problemDetails.Detail = "An unexpected error occurred. Please try again later.";
                break;
        }

        return problemDetails;
    }
}
