// Features/Auth/Commands/Login/LoginCommand.cs
using MediatR;
using InventoryManagementSystem.Common.Models;
using InventoryManagementSystem.Features.Auth.Commands.Register;

namespace InventoryManagementSystem.Features.Auth.Commands.Login
{
    public sealed record LoginCommand(
        string Username,
        string Password
    ) : IRequest<Result<AuthResponse>>;
}
