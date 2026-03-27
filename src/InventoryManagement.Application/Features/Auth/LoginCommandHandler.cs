// ============================================================
// LoginCommandHandler
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryManagement.Application.Features.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<LoginCommandHandler> _logger;

        public LoginCommandHandler(IUnitOfWork uow, IJwtService jwtService,
            IRefreshTokenService refreshTokenService, ILogger<LoginCommandHandler> logger)
        { _uow = uow; _jwtService = jwtService; _refreshTokenService = refreshTokenService; _logger = logger; }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
        {
            User? user = await _uow.Users.GetByUsernameAsync(request.Dto.Username, ct);
            if (user == null)
                return Result<AuthResponseDto>.Unauthorized("Invalid username or password.");

            if (!VerifyPassword(request.Dto.Password, user.PasswordHash))
                return Result<AuthResponseDto>.Unauthorized("Invalid username or password.");

            string accessToken = _jwtService.GenerateAccessToken(user);
            string refreshToken = await _refreshTokenService.GenerateAndStoreRefreshTokenAsync(user, ct);
            _logger.LogInformation("User logged in: {Username}", user.Username);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = accessToken, RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
                Username = user.Username, Email = user.Email, Role = user.Role.ToString()
            });
        }

        private static bool VerifyPassword(string password, string hash)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes) == hash;
        }
    }
}
