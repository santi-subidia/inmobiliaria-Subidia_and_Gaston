// Repositories/UsuarioRepository.cs
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using MySqlConnector;

namespace Inmobiliaria.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IMySqlConnectionFactory _cf;
        public UsuarioRepository(IMySqlConnectionFactory cf) => _cf = cf;

        public async Task<IEnumerable<Usuario>> GetAllAsync()
        {
            var list = new List<Usuario>();
            using var conn = _cf.CreateConnection();
            await conn.OpenAsync();
            const string sql = "SELECT * FROM usuarios ORDER BY id DESC";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync()) list.Add(Map(r));
            return list;
        }

        public async Task<Usuario?> GetByIdAsync(long id)
        {
            using var conn = _cf.CreateConnection();
            await conn.OpenAsync();
            const string sql = "SELECT * FROM usuarios WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? Map(r) : null;
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            using var conn = _cf.CreateConnection();
            await conn.OpenAsync();
            const string sql = "SELECT * FROM usuarios WHERE email=@e LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", email);
            using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? Map(r) : null;
        }

        public async Task<bool> ExistsByEmailAsync(string email, long? excludeId = null)
        {
            using var conn = _cf.CreateConnection();
            await conn.OpenAsync();
            var sql = "SELECT COUNT(*) FROM usuarios WHERE email=@e";
            if (excludeId.HasValue) sql += " AND id<>@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", email);
            if (excludeId.HasValue) cmd.Parameters.AddWithValue("@id", excludeId.Value);
            var n = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return n > 0;
        }

        public async Task<long> CreateAsync(Usuario u)
        {
            using var conn = _cf.CreateConnection();
            await conn.OpenAsync();
            const string sql = @"
INSERT INTO usuarios (email, password_hash, nombre, apellido, telefono, avatar_url, rol_id, is_active, created_at, updated_at)
VALUES (@e,@ph,@n,@a,@t,@av,@rol,@act,@ca,@ua);
SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@e", u.Email);
            cmd.Parameters.AddWithValue("@ph", u.PasswordHash);
            cmd.Parameters.AddWithValue("@n", u.Nombre);
            cmd.Parameters.AddWithValue("@a", u.Apellido);
            cmd.Parameters.AddWithValue("@t", (object?)u.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@av", (object?)u.AvatarUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@rol", u.RolId);
            cmd.Parameters.AddWithValue("@act", u.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@ca", u.CreatedAt == default ? DateTime.UtcNow : u.CreatedAt);
            cmd.Parameters.AddWithValue("@ua", u.UpdatedAt == default ? DateTime.UtcNow : u.UpdatedAt);
            var id = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(id);
        }

        public async Task<bool> UpdateAsync(Usuario u)
        {
            using var conn = _cf.CreateConnection();
            await conn.OpenAsync();
            const string sql = @"
UPDATE usuarios SET
  email=@e, password_hash=@ph, nombre=@n, apellido=@a,
  telefono=@t, avatar_url=@av, rol_id=@rol, is_active=@act, updated_at=@ua
WHERE id=@id;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", u.Id);
            cmd.Parameters.AddWithValue("@e", u.Email);
            cmd.Parameters.AddWithValue("@ph", u.PasswordHash);
            cmd.Parameters.AddWithValue("@n", u.Nombre);
            cmd.Parameters.AddWithValue("@a", u.Apellido);
            cmd.Parameters.AddWithValue("@t", (object?)u.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@av", (object?)u.AvatarUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@rol", u.RolId);
            cmd.Parameters.AddWithValue("@act", u.IsActive ? 1 : 0);
            cmd.Parameters.AddWithValue("@ua", DateTime.UtcNow);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var conn = _cf.CreateConnection();
            await conn.OpenAsync();
            const string sql = "DELETE FROM usuarios WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        private static Usuario Map(MySqlDataReader r) => new()
        {
            Id          = r.GetInt64("id"),
            Email       = r.GetString("email"),
            PasswordHash= r.GetString("password_hash"),
            Nombre      = r.GetString("nombre"),
            Apellido    = r.GetString("apellido"),
            Telefono    = r.IsDBNull(r.GetOrdinal("telefono")) ? null : r.GetString("telefono"),
            AvatarUrl   = r.IsDBNull(r.GetOrdinal("avatar_url")) ? null : r.GetString("avatar_url"),
            RolId       = r.GetInt32("rol_id"),
            IsActive    = r.GetBoolean("is_active"),
            CreatedAt   = r.GetDateTime("created_at"),
            UpdatedAt   = r.GetDateTime("updated_at"),
        };
    }
}
