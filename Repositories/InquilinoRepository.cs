using Inmobiliaria.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using Inmobiliaria.Data;

namespace Inmobiliaria.Repositories
{
    public class InquilinoRepository : IInquilinoRepository
    {
        private readonly IMySqlConnectionFactory _connectionFactory;
        public InquilinoRepository(IMySqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        public async Task<IEnumerable<Inquilino>> GetAllAsync()
        {
            var list = new List<Inquilino>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = "SELECT * FROM inquilinos WHERE fecha_eliminacion IS NULL";
            var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(MapInquilino(reader));
            }
            return list;
        }

        public async Task<Inquilino?> GetByIdAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = "SELECT * FROM inquilinos WHERE id=@id AND fecha_eliminacion IS NULL";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapInquilino(reader);
            return null;
        }

        public async Task<Inquilino?> GetByDniAsync(string dni)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = "SELECT * FROM inquilinos WHERE dni=@dni";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dni", dni);
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapInquilino(reader);
            return null;
        }

        public async Task<(IEnumerable<Inquilino> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
        {
            var list = new List<Inquilino>();

            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            // OFFSET = (page-1)*pageSize
            string sql = @"
        SELECT SQL_CALC_FOUND_ROWS *
        FROM inquilinos
        WHERE fecha_eliminacion IS NULL
        ORDER BY id
        LIMIT @pageSize OFFSET @offset;
        SELECT FOUND_ROWS();";  // devuelve el total ignorando el LIMIT

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@pageSize", pageSize);
            cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);

            using var reader = await cmd.ExecuteReaderAsync();

            // --- 1° resultado: la página de datos ---
            while (await reader.ReadAsync())
            {
                list.Add(MapInquilino(reader));
            }

            // --- 2° resultado: total de registros ---
            await reader.NextResultAsync(); // pasamos al 2do SELECT
            int total = 0;
            if (await reader.ReadAsync())
            {
                total = reader.GetInt32(0);
            }

            return (list, total);
        }


        public async Task<long> CreateAsync(Inquilino i)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = @"INSERT INTO inquilinos (dni, apellido, nombre, telefono, email, direccion, created_at, updated_at, fecha_eliminacion) 
                        VALUES (@dni, @apellido, @nombre, @telefono, @email, @direccion, @created_at, @updated_at, @fecha_eliminacion); 
                        SELECT LAST_INSERT_ID();";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dni", i.Dni);
            cmd.Parameters.AddWithValue("@apellido", i.Apellido);
            cmd.Parameters.AddWithValue("@nombre", i.Nombre);
            cmd.Parameters.AddWithValue("@telefono", i.Telefono ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@email", i.Email);
            cmd.Parameters.AddWithValue("@direccion", i.Direccion);
            cmd.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@fecha_eliminacion", (object)DBNull.Value);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateFechaEliminacionAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = "UPDATE inquilinos SET fecha_eliminacion = @fecha_eliminacion WHERE id=@id";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fecha_eliminacion", DBNull.Value);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateAsync(Inquilino i)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            using var transaction = await conn.BeginTransactionAsync();

            try
            {
                var dniOriginal = await GetDniByIdAsync(i.Id, conn, transaction);

                // Actualizar inquilinos
                var sql = @"
            UPDATE inquilinos 
            SET dni=@dni, apellido=@apellido, nombre=@nombre, 
                telefono=@telefono, email=@email, direccion=@direccion, 
                updated_at=@updated_at
            WHERE id=@id;
            
            UPDATE propietarios 
            SET dni=@dni, apellido=@apellido, nombre=@nombre,
                telefono=@telefono, email=@email, direccion_contacto=@direccion,
                updated_at=@updated_at
            WHERE dni=@dni_original AND EXISTS (SELECT 1 FROM propietarios WHERE dni=@dni_original)";

                var cmd = new MySqlCommand(sql, conn, transaction);
                cmd.Parameters.AddWithValue("@id", i.Id);
                cmd.Parameters.AddWithValue("@dni", i.Dni);
                cmd.Parameters.AddWithValue("@dni_original", dniOriginal);
                cmd.Parameters.AddWithValue("@apellido", i.Apellido);
                cmd.Parameters.AddWithValue("@nombre", i.Nombre);
                cmd.Parameters.AddWithValue("@telefono", i.Telefono ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@email", i.Email);
                cmd.Parameters.AddWithValue("@direccion", i.Direccion);
                cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

                var rows = await cmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();

                // Éxito si al menos se actualizó inquilinos (rows >= 1)
                return rows >= 1;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<string?> GetDniByIdAsync(int id, MySqlConnection conn, MySqlTransaction transaction)
        {
            var sql = "SELECT dni FROM inquilinos WHERE id = @id";
            var cmd = new MySqlCommand(sql, conn, transaction);
            cmd.Parameters.AddWithValue("@id", id);

            var result = await cmd.ExecuteScalarAsync();
            if (result == null || result == DBNull.Value || string.IsNullOrEmpty(result.ToString()))
            {
                throw new Exception($"No se encontró un inquilino con ID {id}");
            }
            return result.ToString();
        }

        public async Task<bool> DeleteAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = "UPDATE inquilinos SET fecha_eliminacion = NOW() WHERE id=@id";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        private static Inquilino MapInquilino(MySqlDataReader reader) => new()
        {
            Id = reader.GetInt32("id"),
            Dni = reader.GetString("dni"),
            Apellido = reader.GetString("apellido"),
            Nombre = reader.GetString("nombre"),
            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString("telefono"),
            Email = reader.GetString("email"),
            Direccion = reader.GetString("direccion"),
            CreatedAt = reader.GetDateTime("created_at"),
            UpdatedAt = reader.GetDateTime("updated_at"),
            FechaEliminacion = reader.IsDBNull(reader.GetOrdinal("fecha_eliminacion")) ? null : reader.GetDateTime("fecha_eliminacion")
        };
    }
}
