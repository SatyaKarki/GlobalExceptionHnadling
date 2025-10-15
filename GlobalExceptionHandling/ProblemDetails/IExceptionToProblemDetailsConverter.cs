using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionHandling.ProblemDetails;

/// <summary>
/// Interface for converting exceptions to ProblemDetails.
/// </summary>
public interface IExceptionToProblemDetailsConverter
{
    /// <summary>
    /// Converts an exception to a ProblemDetails object.
    /// </summary>
    /// <param name="exception">The exception to convert.</param>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A ProblemDetails object representing the exception.</returns>
    Microsoft.AspNetCore.Mvc.ProblemDetails Convert(Exception exception, HttpContext context);
}
