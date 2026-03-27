// ============================================================
// FILE: src/InventoryManagement.API/Controllers/AuthController.cs
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Features.Auth;
using InventoryManagement.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers
{
    public class AuthController : BaseApiController
    {
        /// <summary>Register a new user.</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
            => FromResult(await Mediator.Send(new RegisterUserCommand(dto)));

        /// <summary>Login and receive JWT tokens.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
            => FromResult(await Mediator.Send(new LoginCommand(dto)));

        /// <summary>Refresh an expired access token.</summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
            => FromResult(await Mediator.Send(new RefreshTokenCommand(dto)));

        /// <summary>Revoke a user's refresh token (logout).</summary>
        [HttpPost("revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke()
        {
            string? userId = User.FindFirst(Constants.ClaimTypes.UserId)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            return FromResult(await Mediator.Send(new RevokeTokenCommand(userId)));
        }
    }
}
