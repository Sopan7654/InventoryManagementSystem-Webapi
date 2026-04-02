// Features/Auth/Commands/Logout/LogoutCommandHandler.cs
using System.IdentityModel.Tokens.Jwt;
using MediatR;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Features.Auth.Commands.Logout
{
    public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly ICacheService _cache;

        public LogoutCommandHandler(ICacheService cache) => _cache = cache;

        public async Task<Result<bool>> Handle(LogoutCommand cmd, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(cmd.Token))
                return Result<bool>.Failure("Token provides is empty");

            var tokenHandler = new JwtSecurityTokenHandler();
            if (!tokenHandler.CanReadToken(cmd.Token))
                return Result<bool>.Failure("Invalid token format");

            var jwt = tokenHandler.ReadJwtToken(cmd.Token);
            
            var jti = jwt.Id; // The unique token ID
            if (string.IsNullOrEmpty(jti))
                return Result<bool>.Success(true); // Nothing to blacklist

            var expiresAt = jwt.ValidTo;
            var timeRemaining = expiresAt - DateTime.UtcNow;

            if (timeRemaining > TimeSpan.Zero)
            {
                // Push the JTI to Redis. Expiry = exactly when the token itself natively expires.
                await _cache.SetAsync($"jwt_blacklist_{jti}", true, absoluteExpireTime: timeRemaining, ct: ct);
            }

            return Result<bool>.Success(true);
        }
    }
}
