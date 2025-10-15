namespace GlobalExceptionHandling.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// </summary>
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message, 404)
    {
    }

    public NotFoundException(string resourceName, object key) 
        : base($"{resourceName} with id '{key}' was not found.", 404)
    {
    }
}
