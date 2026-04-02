// Features/Auth/Commands/Logout/LogoutCommand.cs
using MediatR;
using InventoryManagementSystem.Common.Models;

namespace InventoryManagementSystem.Features.Auth.Commands.Logout
{
    public sealed record LogoutCommand(string Token) : IRequest<Result<bool>>;
}
