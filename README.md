# Global Exception Handling in ASP.NET Core

Production-Grade Global Exception Handling in ASP.NET Core — An Architect's Guide

This project demonstrates a robust, production-ready approach to handling exceptions in ASP.NET Core Web API applications using .NET 9.

## 🎯 Features

- ✅ Centralized exception handling using middleware
- ✅ RFC 7807 compliant ProblemDetails responses
- ✅ Correlation ID tracking for distributed tracing
- ✅ Custom exception types for different scenarios
- ✅ Structured logging integration
- ✅ Development-friendly error details (stack traces in dev mode)
- ✅ Production-safe error messages (no sensitive data exposure)
- ✅ Extensible architecture for custom exception converters

## 📁 Project Structure

```
GlobalExceptionHandling/
├── Controllers/
│   ├── ProductsController.cs      # Sample controller demonstrating exceptions
│   └── UsersController.cs         # Sample controller demonstrating correlation ID
├── Middleware/
│   ├── CorrelationIdMiddleware.cs # Generates/propagates correlation IDs
│   └── ExceptionHandlingMiddleware.cs # Catches and formats exceptions
├── Exceptions/
│   ├── AppException.cs            # Base application exception
│   ├── NotFoundException.cs       # 404 Not Found exception
│   └── ValidationFailureException.cs # 400 Validation error exception
├── ProblemDetails/
│   ├── IExceptionToProblemDetailsConverter.cs # Converter interface
│   └── DefaultExceptionToProblemDetailsConverter.cs # Default converter
├── Extensions/
│   ├── ServiceCollectionExtensions.cs # DI configuration
│   └── ApplicationBuilderExtensions.cs # Middleware configuration
├── Program.cs
└── appsettings.json
```

## 🚀 Getting Started

### Prerequisites

- .NET 9 SDK
- Your favorite IDE (Visual Studio, VS Code, Rider)

### Running the Application

1. Clone the repository:
```bash
git clone https://github.com/SatyaKarki/GlobalExceptionHnadling.git
cd GlobalExceptionHnadling/GlobalExceptionHandling
```

2. Build the project:
```bash
dotnet build
```

3. Run the application:
```bash
dotnet run
```

The API will be available at `http://localhost:5254` (or the port shown in your console).

## 📝 Usage Examples

### 1. Correlation ID Tracking

Every request automatically gets a correlation ID that can be used for distributed tracing:

```bash
# Server generates correlation ID if not provided
curl http://localhost:5254/api/users/correlation

# Or provide your own correlation ID
curl -H "X-Correlation-Id: my-custom-id" http://localhost:5254/api/users/correlation
```

Response:
```json
{
  "correlationId": "my-custom-id"
}
```

### 2. Validation Errors (400 Bad Request)

```bash
curl http://localhost:5254/api/products/-5
```

Response:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Validation Error",
  "status": 400,
  "detail": "Validation failure occurred.",
  "instance": "/api/products/-5",
  "correlationId": "4e5c5b80-77c8-49a7-b4dd-12bca5377189",
  "errors": {
    "id": ["Product ID must be greater than 0"]
  }
}
```

### 3. Resource Not Found (404 Not Found)

```bash
curl http://localhost:5254/api/products/999
```

Response:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Resource Not Found",
  "status": 404,
  "detail": "Product with id '999' was not found.",
  "instance": "/api/products/999",
  "correlationId": "8df29550-4c02-4f8a-a86a-77b74b693dd1"
}
```

### 4. Custom Application Error (503 Service Unavailable)

```bash
curl http://localhost:5254/api/products/error
```

Response:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "Application Error",
  "status": 503,
  "detail": "A custom application error occurred",
  "instance": "/api/products/error",
  "correlationId": "546de90c-b3c4-4179-9b31-a04fa3e09df1"
}
```

### 5. Unhandled Exception (500 Internal Server Error)

```bash
curl http://localhost:5254/api/products/unhandled
```

Response:
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "An unexpected error occurred. Please try again later.",
  "instance": "/api/products/unhandled",
  "correlationId": "78d0a3c8-0457-416d-ae48-350dd015bb66"
}
```

**Note:** In development mode, responses include additional debugging information like `exception`, `stackTrace`, and `innerException`.

## 🏗️ Architecture & Design

### Middleware Pipeline

The middleware components are registered in the following order:

1. **CorrelationIdMiddleware** - Ensures every request has a correlation ID
2. **ExceptionHandlingMiddleware** - Catches all unhandled exceptions

```csharp
// In Program.cs
app.UseGlobalExceptionHandling(); // Registers both middleware
```

### Exception Hierarchy

```
Exception
  └── AppException (base, 500)
      ├── NotFoundException (404)
      └── ValidationFailureException (400)
```

### Creating Custom Exceptions

```csharp
// Simple custom exception
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) 
        : base(message, 401)
    {
    }
}

// Usage in controller
throw new UnauthorizedException("Invalid credentials");
```

### Custom ProblemDetails Converter

You can create custom converters for specific exception types:

```csharp
public class CustomExceptionConverter : IExceptionToProblemDetailsConverter
{
    public ProblemDetails Convert(Exception exception, HttpContext context)
    {
        // Custom conversion logic
    }
}

// Register in Program.cs
builder.Services.AddSingleton<IExceptionToProblemDetailsConverter, CustomExceptionConverter>();
```

## 🔍 Key Components

### CorrelationIdMiddleware

- Generates or extracts correlation IDs from request headers
- Stores correlation ID in `HttpContext.Items` for use throughout the request
- Adds correlation ID to response headers for client tracking

### ExceptionHandlingMiddleware

- Catches all unhandled exceptions
- Logs exceptions with correlation ID for traceability
- Converts exceptions to RFC 7807 compliant ProblemDetails
- Includes stack traces in development environment only
- Ensures consistent error response format

### Exception Classes

- **AppException**: Base exception with customizable status code
- **NotFoundException**: For resource not found scenarios (404)
- **ValidationFailureException**: For validation errors with field-level details (400)

## 🛡️ Production Considerations

### Security

- ❌ **Never** expose sensitive data in error messages
- ✅ Stack traces only shown in development environment
- ✅ Generic error messages for unexpected exceptions in production
- ✅ Correlation IDs for secure error tracking

### Performance

- ✅ Middleware is highly performant
- ✅ Exception handling only kicks in when exceptions occur
- ✅ Logging is async and non-blocking

### Monitoring & Observability

- ✅ All exceptions are logged with structured logging
- ✅ Correlation IDs enable end-to-end request tracking
- ✅ Compatible with Application Insights, Serilog, etc.

## 📚 Best Practices

1. **Use specific exception types** - Create custom exceptions for different business scenarios
2. **Include correlation IDs** - Essential for debugging in distributed systems
3. **Log at the right level** - Use appropriate log levels (Error for exceptions)
4. **Don't catch what you can't handle** - Let the middleware handle unexpected exceptions
5. **Validate early** - Validate input as early as possible in your controllers
6. **Use ProblemDetails** - Follow RFC 7807 for consistent API error responses

## 🧪 Testing

You can test various exception scenarios using the provided endpoints:

- `GET /api/products/{id}` - Returns product or NotFoundException
- `GET /api/products/{id}` (with negative ID) - ValidationFailureException
- `POST /api/products` - Demonstrates validation with multiple fields
- `GET /api/products/error` - Custom AppException
- `GET /api/products/unhandled` - Unhandled exception
- `GET /api/users/correlation` - View current correlation ID

## 📖 Additional Resources

- [RFC 7807 - Problem Details for HTTP APIs](https://tools.ietf.org/html/rfc7807)
- [ASP.NET Core Middleware](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/middleware/)
- [Exception Handling in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/error-handling)

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 👤 Author

**Satya Karki**

---

⭐ If you find this project helpful, please give it a star!
