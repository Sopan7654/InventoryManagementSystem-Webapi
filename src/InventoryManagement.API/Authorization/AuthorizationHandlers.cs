// ============================================================
// FILE: src/InventoryManagement.API/Authorization/AuthorizationHandlers.cs
// ============================================================
using InventoryManagement.Shared;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagement.API.Authorization
{
    /// <summary>Requirement that user has a minimum experience level.</summary>
    public class MinimumExperienceRequirement : IAuthorizationRequirement
    {
        public string RequiredLevel { get; }
        public MinimumExperienceRequirement(string requiredLevel) { RequiredLevel = requiredLevel; }
    }

    public class MinimumExperienceHandler : AuthorizationHandler<MinimumExperienceRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            MinimumExperienceRequirement requirement)
        {
            string? experience = context.User.FindFirst(Constants.ClaimTypes.Experience)?.Value;
            if (experience != null && string.Equals(experience, requirement.RequiredLevel, StringComparison.OrdinalIgnoreCase))
                context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    /// <summary>Requirement that user has access to a specific warehouse.</summary>
    public class WarehouseAccessRequirement : IAuthorizationRequirement { }

    public class WarehouseAccessHandler : AuthorizationHandler<WarehouseAccessRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public WarehouseAccessHandler(IHttpContextAccessor httpContextAccessor) { _httpContextAccessor = httpContextAccessor; }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            WarehouseAccessRequirement requirement)
        {
            string? access = context.User.FindFirst(Constants.ClaimTypes.WarehouseAccess)?.Value;
            if (string.IsNullOrEmpty(access)) return Task.CompletedTask;

            // Admin bypass
            if (context.User.IsInRole(Constants.Roles.Admin)) { context.Succeed(requirement); return Task.CompletedTask; }

            string? warehouseId = _httpContextAccessor.HttpContext?.Request.Query["warehouseId"].FirstOrDefault();
            if (warehouseId != null && access.Split(',').Contains(warehouseId))
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
