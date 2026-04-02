// Infrastructure/Middleware/JwtBlacklistMiddleware.cs
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using System.Net;
using InventoryManagementSystem.Common.Interfaces;

namespace InventoryManagementSystem.Infrastructure.Middleware
{
    /// <summary>
    /// Middleware that checks incoming JWT tokens against the Redis blacklist.
    /// Runs very early in the pipeline before MVC Authentication filters.
    /// </summary>
    public sealed class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ICacheService cache)
        {
            var authHeader = context.Request.Headers.Authorization.ToString();
            
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                var handler = new JwtSecurityTokenHandler();

                if (handler.CanReadToken(token))
                {
                    try 
                    {
                        var jwt = handler.ReadJwtToken(token);
                        var jti = jwt.Id;

                        if (!string.IsNullOrEmpty(jti))
                        {
                            var isBlacklisted = await cache.ExistsAsync($"jwt_blacklist_{jti}", context.RequestAborted);
                            if (isBlacklisted)
                            {
                                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                                context.Response.ContentType = "application/json";
                                await context.Response.WriteAsync("{\"error\": \"Token has been revoked.\"}");
                                return; // Short-circuit pipeline
                            }
                        }
                    }
                    catch 
                    {
                        // Token cannot be read (malformed). We let standard authorization handle it.
                    }
                }
            }

            await _next(context);
        }
    }
}
