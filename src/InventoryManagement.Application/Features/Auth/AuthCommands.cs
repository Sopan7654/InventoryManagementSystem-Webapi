// ============================================================
// Auth Commands & Queries
// ============================================================
using InventoryManagement.Application.DTOs;
using InventoryManagement.Shared;
using MediatR;

namespace InventoryManagement.Application.Features.Auth
{
    public record RegisterUserCommand(RegisterDto Dto) : IRequest<Result<AuthResponseDto>>;
    public record LoginCommand(LoginDto Dto) : IRequest<Result<AuthResponseDto>>;
    public record RefreshTokenCommand(RefreshTokenDto Dto) : IRequest<Result<AuthResponseDto>>;
    public record RevokeTokenCommand(string UserId) : IRequest<Result<bool>>;
}
