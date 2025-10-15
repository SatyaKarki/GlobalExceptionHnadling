namespace GlobalExceptionHandling.Exceptions;

/// <summary>
/// Base application exception class that all custom exceptions should inherit from.
/// </summary>
public class AppException : Exception
{
    public int StatusCode { get; set; }

    public AppException(string message, int statusCode = 500) : base(message)
    {
        StatusCode = statusCode;
    }

    public AppException(string message, Exception innerException, int statusCode = 500) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
