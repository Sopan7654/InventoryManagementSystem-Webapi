// Features/Auth/Commands/Register/RegisterCommand.cs
using MediatR;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Features.Auth.Commands.Register
{
    public sealed record RegisterCommand(
        string Username,
        string Email,
        string Password
    ) : IRequest<Result<AuthResponse>>;

    public sealed record AuthResponse(
        string UserId,
        string Username,
        string Email,
        string Role,
        string Token
    );
}
