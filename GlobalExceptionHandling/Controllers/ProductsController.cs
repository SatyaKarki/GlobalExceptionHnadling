using GlobalExceptionHandling.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace GlobalExceptionHandling.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(ILogger<ProductsController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets a product by ID. Throws NotFoundException if not found.
    /// </summary>
    [HttpGet("{id}")]
    public IActionResult GetProduct(int id)
    {
        _logger.LogInformation("Getting product with ID: {ProductId}", id);

        if (id <= 0)
        {
            throw new ValidationFailureException("id", "Product ID must be greater than 0");
        }

        if (id > 100)
        {
            throw new NotFoundException("Product", id);
        }

        return Ok(new { Id = id, Name = $"Product {id}", Price = 99.99 });
    }

    /// <summary>
    /// Creates a new product. Demonstrates validation failure.
    /// </summary>
    [HttpPost]
    public IActionResult CreateProduct([FromBody] CreateProductRequest request)
    {
        _logger.LogInformation("Creating product: {ProductName}", request.Name);

        var errors = new Dictionary<string, string[]>();

        if (string.IsNullOrWhiteSpace(request.Name))
        {
            errors.Add("name", new[] { "Product name is required" });
        }

        if (request.Price <= 0)
        {
            errors.Add("price", new[] { "Price must be greater than 0" });
        }

        if (errors.Any())
        {
            throw new ValidationFailureException(errors);
        }

        return CreatedAtAction(nameof(GetProduct), new { id = 1 }, request);
    }

    /// <summary>
    /// Demonstrates a generic application exception.
    /// </summary>
    [HttpGet("error")]
    public IActionResult TriggerError()
    {
        throw new AppException("A custom application error occurred", 503);
    }

    /// <summary>
    /// Demonstrates an unhandled exception.
    /// </summary>
    [HttpGet("unhandled")]
    public IActionResult TriggerUnhandledException()
    {
        throw new InvalidOperationException("This is an unhandled exception for testing");
    }
}

public record CreateProductRequest(string Name, decimal Price);
