// Features/Auth/AuthController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using InventoryManagementSystem.Features.Auth.Commands.Login;
using InventoryManagementSystem.Features.Auth.Commands.Register;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Features.Auth
{
    [ApiController]
    [Route("api/auth")]
    [Produces("application/json")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        /// <summary>Register a new user account.</summary>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Register", Description = "Creates a new user account and returns a JWT token.")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 422)]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req, CancellationToken ct)
        {
            var result = await _mediator.Send(
                new RegisterCommand(req.Username, req.Email, req.Password), ct);
            return Ok(result);
        }

        /// <summary>Authenticate with username and password.</summary>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "Login", Description = "Authenticates a user and returns a JWT token.")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req, CancellationToken ct)
        {
            var result = await _mediator.Send(
                new LoginCommand(req.Username, req.Password), ct);
            return Ok(result);
        }

        /// <summary>Logout user by blacklisting their token.</summary>
        [HttpPost("logout")]
        [SwaggerOperation(Summary = "Logout", Description = "Revokes the active JWT token by sending it to the Redis blacklist.")]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            var authHeader = Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return Ok(Result<bool>.Success(true)); // Already detached

            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            var result = await _mediator.Send(new InventoryManagementSystem.Features.Auth.Commands.Logout.LogoutCommand(token), ct);
            return Ok(result);
        }
    }

    // ── Request DTOs ──────────────────────────────────────────────────────────
    public sealed record RegisterRequest(string Username, string Email, string Password);
    public sealed record LoginRequest(string Username, string Password);
}
