namespace GlobalExceptionHandling.Exceptions;

/// <summary>
/// Exception thrown when validation fails.
/// </summary>
public class ValidationFailureException : AppException
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationFailureException(IDictionary<string, string[]> errors) 
        : base("One or more validation failures have occurred.", 400)
    {
        Errors = errors;
    }

    public ValidationFailureException(string field, string error) 
        : base("Validation failure occurred.", 400)
    {
        Errors = new Dictionary<string, string[]>
        {
            { field, new[] { error } }
        };
    }
}
