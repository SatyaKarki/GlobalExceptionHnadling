using GlobalExceptionHandling.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionHandling.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;

    public UsersController(ILogger<UsersController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets a user by ID. Demonstrates NotFoundException.
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetUser(string id)
    {
        _logger.LogInformation("Getting user with ID: {UserId}", id);

        if (string.IsNullOrWhiteSpace(id))
        {
            throw new ValidationFailureException("id", "User ID cannot be empty");
        }

        // Simulate user not found
        if (id != "123")
        {
            throw new NotFoundException("User", id);
        }

        return Ok(new { Id = id, Name = "John Doe", Email = "john.doe@example.com" });
    }

    /// <summary>
    /// Gets user correlation ID from the current request.
    /// </summary>
    [HttpGet("correlation")]
    public IActionResult GetCorrelationId()
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString() ?? "N/A";
        return Ok(new { CorrelationId = correlationId });
    }
}
