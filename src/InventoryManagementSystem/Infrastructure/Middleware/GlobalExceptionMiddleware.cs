// Infrastructure/Middleware/GlobalExceptionMiddleware.cs
using System.Net;
using System.Text.Json;
using InventoryManagementSystem.Common.Exceptions;
using ValidationException = InventoryManagementSystem.Common.Exceptions.ValidationException;

namespace InventoryManagementSystem.Infrastructure.Middleware
{
    /// <summary>
    /// Global exception handler middleware — translates domain exceptions to
    /// clean RFC 7807 Problem Details JSON responses.
    /// Maps: NotFoundException → 404, ValidationException → 400, DomainException → 422, else 500.
    /// </summary>
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public GlobalExceptionMiddleware(RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title, errors) = exception switch
            {
                NotFoundException nfe  => (HttpStatusCode.NotFound,
                                            "Resource Not Found",
                                            (object?)new { message = nfe.Message }),

                ValidationException ve => (HttpStatusCode.BadRequest,
                                            "Validation Failed",
                                            (object?)new { errors = ve.Errors }),

                DomainException de     => (HttpStatusCode.UnprocessableEntity,
                                            "Business Rule Violation",
                                            (object?)new { message = de.Message }),

                _                      => (HttpStatusCode.InternalServerError,
                                            "Internal Server Error",
                                            (object?)new { message = "An unexpected error occurred." })
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, "Unhandled server error");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)statusCode;

            var body = JsonSerializer.Serialize(new
            {
                success = false,
                status  = (int)statusCode,
                title,
                detail  = errors
            }, _jsonOptions);

            await context.Response.WriteAsync(body);
        }
    }
}
