// ============================================================
// FILE: src/InventoryManagement.API/Middleware/ExceptionHandlingMiddleware.cs
// ============================================================
using System.Net;
using System.Text.Json;
using FluentValidation;

namespace InventoryManagement.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        { _next = next; _logger = logger; }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation error: {Message}", ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    type = "ValidationError",
                    title = "One or more validation errors occurred.",
                    status = 400,
                    errors
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    type = "ServerError",
                    title = "An unexpected error occurred.",
                    status = 500,
                    detail = context.RequestServices.GetRequiredService<IHostEnvironment>().IsDevelopment()
                        ? ex.Message : "Please contact support."
                }));
            }
        }
    }
}
