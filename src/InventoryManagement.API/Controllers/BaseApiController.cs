// ============================================================
// FILE: src/InventoryManagement.API/Controllers/BaseApiController.cs
// ============================================================
using InventoryManagement.Shared;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    /// <summary>
    /// Base controller providing Result<T> to IActionResult mapping.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public abstract class BaseApiController : ControllerBase
    {
        private IMediator? _mediator;
        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

        /// <summary>Maps a Result<T> to the appropriate HTTP status code.</summary>
        protected IActionResult FromResult<T>(Result<T> result) => result.IsSuccess
            ? Ok(result.Value)
            : result.ErrorType switch
            {
                ResultError.NotFound => NotFound(new { error = result.Error }),
                ResultError.Conflict => Conflict(new { error = result.Error }),
                ResultError.Validation => BadRequest(new { error = result.Error, errors = result.ValidationErrors }),
                ResultError.Unauthorized => Unauthorized(new { error = result.Error }),
                _ => StatusCode(500, new { error = result.Error })
            };

        /// <summary>Maps a Result<T> to 201 Created with a location header.</summary>
        protected IActionResult CreatedFromResult<T>(Result<T> result, string routeName, object routeValues)
        {
            if (!result.IsSuccess) return FromResult(result);
            return CreatedAtRoute(routeName, routeValues, result.Value);
        }
    }
}
