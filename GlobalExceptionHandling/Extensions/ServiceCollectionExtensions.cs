using GlobalExceptionHandling.ProblemDetails;

namespace GlobalExceptionHandling.Extensions;

/// <summary>
/// Extension methods for registering global exception handling services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds global exception handling services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddGlobalExceptionHandling(this IServiceCollection services)
    {
        services.AddSingleton<IExceptionToProblemDetailsConverter, DefaultExceptionToProblemDetailsConverter>();
        return services;
    }
}
