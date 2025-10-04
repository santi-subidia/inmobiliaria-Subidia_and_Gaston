using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using MySqlConnector;

namespace Inmobiliaria.Repositories
{
    public class TipoInmuebleRepository : ITipoInmuebleRepository
    {
        private readonly IMySqlConnectionFactory _connectionFactory;
        public TipoInmuebleRepository(IMySqlConnectionFactory connectionFactory)
            => _connectionFactory = connectionFactory;

        public async Task<IEnumerable<TipoInmueble>> GetAllAsync()
        {
            var list = new List<TipoInmueble>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            const string sql = "SELECT * FROM tipos_inmueble WHERE fecha_eliminacion IS NULL";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(Map(reader));
            return list;
        }

        public async Task<TipoInmueble?> GetByIdAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            const string sql = "SELECT * FROM tipos_inmueble WHERE id=@id AND fecha_eliminacion IS NULL";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Map(reader) : null;
        }

        public async Task<TipoInmueble?> GetByNombreAsync(string nombre)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            const string sql = "SELECT * FROM tipos_inmueble WHERE nombre=@nombre";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nombre", nombre);
            using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync() ? Map(reader) : null;
        }

        public async Task<long> CreateAsync(TipoInmueble t)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            const string sql = @"
INSERT INTO tipos_inmueble (nombre, descripcion, activo, created_at, updated_at, fecha_eliminacion)
VALUES (@nombre, @descripcion, @activo, @created_at, @updated_at, @fecha_eliminacion);
SELECT LAST_INSERT_ID();";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@nombre", t.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", (object?)t.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@activo", t.Activo);
            cmd.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@fecha_eliminacion", DBNull.Value);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(TipoInmueble t)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            const string sql = @"
UPDATE tipos_inmueble
SET nombre=@nombre, descripcion=@descripcion, activo=@activo, updated_at=@updated_at
WHERE id=@id AND fecha_eliminacion IS NULL;";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", t.Id);
            cmd.Parameters.AddWithValue("@nombre", t.Nombre);
            cmd.Parameters.AddWithValue("@descripcion", (object?)t.Descripcion ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@activo", t.Activo);
            cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            const string sql = "UPDATE tipos_inmueble SET fecha_eliminacion = NOW() WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateFechaEliminacionAsync(long id)
        {
            // Reactivar (setear NULL si estaba eliminado). Igual patrón que en InquilinoController.ExisteDni
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            const string sql = "UPDATE tipos_inmueble SET fecha_eliminacion = NULL WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<IEnumerable<TipoInmueble>> GetActivosAsync()
        {
            var list = new List<TipoInmueble>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            const string sql = "SELECT * FROM tipos_inmueble WHERE fecha_eliminacion IS NULL AND activo = 1 ORDER BY nombre";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(Map(reader));
            return list;
        }

        private static TipoInmueble Map(MySqlDataReader r) => new()
        {
            Id = r.GetInt32("id"),
            Nombre = r.GetString("nombre"),
            Descripcion = r.IsDBNull(r.GetOrdinal("descripcion")) ? null : r.GetString("descripcion"),
            Activo = r.GetBoolean("activo"),
            CreatedAt = r.GetDateTime("created_at"),
            UpdatedAt = r.GetDateTime("updated_at"),
            FechaEliminacion = r.IsDBNull(r.GetOrdinal("fecha_eliminacion")) ? null : r.GetDateTime("fecha_eliminacion")
        };
    }
}
