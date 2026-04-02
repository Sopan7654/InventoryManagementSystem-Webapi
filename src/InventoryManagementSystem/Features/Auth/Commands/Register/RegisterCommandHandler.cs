// Features/Auth/Commands/Register/RegisterCommandHandler.cs
using MediatR;
using BCrypt.Net;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Domain.Entities;
using InventoryManagementSystem.Features.Auth.Repository;
using InventoryManagementSystem.Features.Auth.Services;

namespace InventoryManagementSystem.Features.Auth.Commands.Register
{
    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponse>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService      _jwt;

        public RegisterCommandHandler(IUserRepository userRepo, IJwtService jwt)
        {
            _userRepo = userRepo;
            _jwt      = jwt;
        }

        public async Task<Result<AuthResponse>> Handle(RegisterCommand cmd, CancellationToken ct)
        {
            // Check for duplicates
            if (await _userRepo.UsernameExistsAsync(cmd.Username, ct))
                throw new DomainException($"Username '{cmd.Username}' is already taken.");

            if (await _userRepo.EmailExistsAsync(cmd.Email, ct))
                throw new DomainException($"Email '{cmd.Email}' is already registered.");

            // Hash password with BCrypt (work factor 12)
            var hash = BCrypt.Net.BCrypt.HashPassword(cmd.Password, workFactor: 12);

            var user = new AppUser
            {
                UserId       = Guid.NewGuid().ToString(),
                Username     = cmd.Username.Trim(),
                Email        = cmd.Email.Trim().ToLowerInvariant(),
                PasswordHash = hash,
                Role         = "User",
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            };

            await _userRepo.InsertAsync(user, ct);

            var token = _jwt.GenerateToken(user);

            return Result<AuthResponse>.Success(new AuthResponse(
                user.UserId,
                user.Username,
                user.Email,
                user.Role,
                token
            ));
        }
    }
}
