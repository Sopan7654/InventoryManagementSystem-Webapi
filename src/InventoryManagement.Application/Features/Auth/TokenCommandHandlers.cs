// ============================================================
// RefreshToken & RevokeToken Handlers
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace InventoryManagement.Application.Features.Auth
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;

        public RefreshTokenCommandHandler(IUnitOfWork uow, IJwtService jwtService,
            IRefreshTokenService refreshTokenService)
        { _uow = uow; _jwtService = jwtService; _refreshTokenService = refreshTokenService; }

        public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            ClaimsPrincipal? principal = _jwtService.GetPrincipalFromExpiredToken(request.Dto.AccessToken);
            if (principal == null)
                return Result<AuthResponseDto>.Unauthorized("Invalid access token.");

            User? user = await _refreshTokenService.ValidateRefreshTokenAsync(request.Dto.RefreshToken, ct);
            if (user == null)
                return Result<AuthResponseDto>.Unauthorized("Invalid or expired refresh token.");

            string newAccess = _jwtService.GenerateAccessToken(user);
            string newRefresh = await _refreshTokenService.GenerateAndStoreRefreshTokenAsync(user, ct);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = newAccess, RefreshToken = newRefresh,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
                Username = user.Username, Email = user.Email, Role = user.Role.ToString()
            });
        }
    }

    public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, Result<bool>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<RevokeTokenCommandHandler> _logger;

        public RevokeTokenCommandHandler(IUnitOfWork uow, IRefreshTokenService refreshTokenService,
            ILogger<RevokeTokenCommandHandler> logger)
        { _uow = uow; _refreshTokenService = refreshTokenService; _logger = logger; }

        public async Task<Result<bool>> Handle(RevokeTokenCommand request, CancellationToken ct)
        {
            User? user = await _uow.Users.GetByIdAsync(request.UserId, ct);
            if (user == null) return Result<bool>.NotFound($"User '{request.UserId}' not found.");

            await _refreshTokenService.RevokeRefreshTokenAsync(user, ct);
            _logger.LogInformation("Token revoked for user: {UserId}", request.UserId);
            return Result<bool>.Success(true);
        }
    }
}
