// ============================================================
// FILE: src/InventoryManagement.Infrastructure/Services/JwtService.cs
// ============================================================
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using InventoryManagement.Application.Interfaces;
using InventoryManagement.Domain.Models;
using InventoryManagement.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace InventoryManagement.Infrastructure.Services
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config) { _config = config; }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.UserId),
                new(JwtRegisteredClaimNames.UniqueName, user.Username),
                new(JwtRegisteredClaimNames.Email, user.Email),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, user.Role.ToString()),
                new(Constants.ClaimTypes.UserId, user.UserId)
            };

            if (!string.IsNullOrEmpty(user.Department))
                claims.Add(new Claim(Constants.ClaimTypes.Department, user.Department));
            if (!string.IsNullOrEmpty(user.Experience))
                claims.Add(new Claim(Constants.ClaimTypes.Experience, user.Experience));
            if (!string.IsNullOrEmpty(user.WarehouseAccess))
                claims.Add(new Claim(Constants.ClaimTypes.WarehouseAccess, user.WarehouseAccess));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _config["JwtSettings:SecretKey"] ?? "DefaultSuperSecretKeyThatIsAtLeast32CharactersLong!"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            int expiryMinutes = int.TryParse(_config["JwtSettings:AccessTokenExpiryMinutes"], out int m) ? m : 15;

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"] ?? "InventoryManagement",
                audience: _config["JwtSettings:Audience"] ?? "InventoryManagementAPI",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            byte[] randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var validationParams = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    _config["JwtSettings:SecretKey"] ?? "DefaultSuperSecretKeyThatIsAtLeast32CharactersLong!")),
                ValidateLifetime = false // allow expired tokens
            };

            try
            {
                ClaimsPrincipal principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, validationParams, out SecurityToken securityToken);

                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    return null;

                return principal;
            }
            catch { return null; }
        }
    }
}
