// Features/Auth/Repository/UserRepository.cs
using MySql.Data.MySqlClient;
using InventoryManagementSystem.Common.Interfaces;
using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Features.Auth.Repository
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly IDbConnectionFactory _factory;
        public UserRepository(IDbConnectionFactory factory) => _factory = factory;

        public async Task<AppUser?> GetByUsernameAsync(string username, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = "SELECT * FROM AppUser WHERE Username = @username AND IsActive = 1 LIMIT 1";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            return await rdr.ReadAsync(ct) ? Map(rdr) : null;
        }

        public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = "SELECT * FROM AppUser WHERE Email = @email AND IsActive = 1 LIMIT 1";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);
            await using var rdr = await cmd.ExecuteReaderAsync(ct);
            return await rdr.ReadAsync(ct) ? Map(rdr) : null;
        }

        public async Task<bool> UsernameExistsAsync(string username, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = "SELECT COUNT(1) FROM AppUser WHERE Username = @username";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@username", username);
            return Convert.ToInt64(await cmd.ExecuteScalarAsync(ct)) > 0;
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = "SELECT COUNT(1) FROM AppUser WHERE Email = @email";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@email", email);
            return Convert.ToInt64(await cmd.ExecuteScalarAsync(ct)) > 0;
        }

        public async Task InsertAsync(AppUser user, CancellationToken ct = default)
        {
            await using var conn = _factory.CreateConnection();
            await conn.OpenAsync(ct);
            const string sql = @"INSERT INTO AppUser
                (UserId, Username, Email, PasswordHash, Role, IsActive, CreatedAt)
                VALUES (@id, @username, @email, @hash, @role, @active, @created)";
            await using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id",       user.UserId);
            cmd.Parameters.AddWithValue("@username", user.Username);
            cmd.Parameters.AddWithValue("@email",    user.Email);
            cmd.Parameters.AddWithValue("@hash",     user.PasswordHash);
            cmd.Parameters.AddWithValue("@role",     user.Role);
            cmd.Parameters.AddWithValue("@active",   user.IsActive);
            cmd.Parameters.AddWithValue("@created",  user.CreatedAt);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        private static AppUser Map(System.Data.Common.DbDataReader r) => new()
        {
            UserId       = r["UserId"].ToString()!,
            Username     = r["Username"].ToString()!,
            Email        = r["Email"].ToString()!,
            PasswordHash = r["PasswordHash"].ToString()!,
            Role         = r["Role"].ToString()!,
            IsActive     = Convert.ToBoolean(r["IsActive"]),
            CreatedAt    = Convert.ToDateTime(r["CreatedAt"])
        };
    }
}
