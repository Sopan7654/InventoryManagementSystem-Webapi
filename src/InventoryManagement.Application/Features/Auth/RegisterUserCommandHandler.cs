// ============================================================
// RegisterUserCommandHandler
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
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponseDto>>
    {
        private readonly IUnitOfWork _uow;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<RegisterUserCommandHandler> _logger;

        public RegisterUserCommandHandler(IUnitOfWork uow, IJwtService jwtService,
            IRefreshTokenService refreshTokenService, ILogger<RegisterUserCommandHandler> logger)
        { _uow = uow; _jwtService = jwtService; _refreshTokenService = refreshTokenService; _logger = logger; }

        public async Task<Result<AuthResponseDto>> Handle(RegisterUserCommand request, CancellationToken ct)
        {
            if (await _uow.Users.UsernameExistsAsync(request.Dto.Username, ct))
                return Result<AuthResponseDto>.ConflictError($"Username '{request.Dto.Username}' already exists.");

            if (await _uow.Users.EmailExistsAsync(request.Dto.Email, ct))
                return Result<AuthResponseDto>.ConflictError($"Email '{request.Dto.Email}' already in use.");

            User user = new()
            {
                Username = request.Dto.Username,
                Email = request.Dto.Email,
                PasswordHash = HashPassword(request.Dto.Password),
                Role = Domain.Enums.UserRole.Viewer,
                Department = request.Dto.Department,
                Experience = request.Dto.Experience,
                WarehouseAccess = request.Dto.WarehouseAccess
            };

            await _uow.Users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            string accessToken = _jwtService.GenerateAccessToken(user);
            string refreshToken = await _refreshTokenService.GenerateAndStoreRefreshTokenAsync(user, ct);
            _logger.LogInformation("User registered: {Username}", user.Username);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = accessToken, RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(15),
                Username = user.Username, Email = user.Email, Role = user.Role.ToString()
            });
        }

        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
