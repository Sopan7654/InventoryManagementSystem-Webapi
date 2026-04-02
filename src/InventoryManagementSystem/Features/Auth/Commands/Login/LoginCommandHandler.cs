// Features/Auth/Commands/Login/LoginCommandHandler.cs
using MediatR;
using InventoryManagementSystem.Common.Exceptions;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Auth.Commands.Register;
using InventoryManagementSystem.Features.Auth.Repository;
using InventoryManagementSystem.Features.Auth.Services;

namespace InventoryManagementSystem.Features.Auth.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtService      _jwt;

        public LoginCommandHandler(IUserRepository userRepo, IJwtService jwt)
        {
            _userRepo = userRepo;
            _jwt      = jwt;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand cmd, CancellationToken ct)
        {
            var user = await _userRepo.GetByUsernameAsync(cmd.Username, ct);

            // Guard: user not found OR wrong password (same message — no user enumeration)
            if (user is null || !BCrypt.Net.BCrypt.Verify(cmd.Password, user.PasswordHash))
                throw new NotFoundException("Invalid username or password.");

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
