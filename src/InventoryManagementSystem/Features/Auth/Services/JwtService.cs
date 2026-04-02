// Features/Auth/Services/JwtService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Auth.Services
{
    /// <summary>
    /// Generates signed HS256 JWT tokens.
    /// Secret key, issuer, audience, and expiry are read from appsettings.json ["Jwt"] section.
    /// </summary>
    public sealed class JwtService : IJwtService
    {
        private readonly IConfiguration _config;

        public JwtService(IConfiguration config) => _config = config;

        public string GenerateToken(AppUser user)
        {
            var key        = _config["Jwt:Key"]
                             ?? throw new InvalidOperationException("JWT Key is not configured.");
            var issuer     = _config["Jwt:Issuer"]     ?? "InventoryManagementSystem";
            var audience   = _config["Jwt:Audience"]   ?? "InventoryManagementSystem";
            var expiryMins = int.Parse(_config["Jwt:ExpiryMinutes"] ?? "480");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,   user.UserId),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role,               user.Role),
                new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer:             issuer,
                audience:           audience,
                claims:             claims,
                notBefore:          DateTime.UtcNow,
                expires:            DateTime.UtcNow.AddMinutes(expiryMins),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
