using Inmobiliaria.Data;
using Inmobiliaria.Models;
using MySqlConnector;

namespace Inmobiliaria.Repositories
{
    public class PropietarioRepository : IPropietarioRepository
    {
        private readonly IMySqlConnectionFactory _connectionFactory;

        public PropietarioRepository(IMySqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // =========================
        // Lecturas
        // =========================
        public async Task<IEnumerable<Propietario>> GetAllAsync()
        {
            // Solo activos (sin fecha_eliminacion)
            var list = new List<Propietario>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"SELECT * FROM propietarios WHERE fecha_eliminacion IS NULL ORDER BY apellido, nombre";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(Map(reader));

            return list;
        }

        public async Task<IEnumerable<Propietario>> GetAllWithFiltersAsync(bool activos)
        {
            var list = new List<Propietario>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            var sql = activos
                ? @"SELECT * FROM propietarios WHERE fecha_eliminacion IS NULL ORDER BY apellido, nombre"
                : @"SELECT * FROM propietarios ORDER BY apellido, nombre";

            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(Map(reader));

            return list;
        }

        public async Task<Propietario?> GetByIdAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"SELECT * FROM propietarios WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Map(reader);

            return null;
        }

        public async Task<Propietario?> GetByDniAsync(string dni)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"SELECT * FROM propietarios WHERE dni=@dni LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dni", dni);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return Map(reader);

            return null;
        }

        // =========================
        // Escrituras
        // =========================
        public async Task<long> CreateAsync(Propietario p)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
INSERT INTO propietarios
(dni, apellido, nombre, telefono, email, fecha_eliminacion)
VALUES
(@dni, @apellido, @nombre, @telefono, @email, @fecha_eliminacion);
SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dni", p.Dni);
            cmd.Parameters.AddWithValue("@apellido", p.Apellido);
            cmd.Parameters.AddWithValue("@nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)p.Email ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@fecha_eliminacion", (object?)p.FechaEliminacion ?? DBNull.Value);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Propietario p)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
UPDATE propietarios SET
  dni                = @dni,
  apellido           = @apellido,
  nombre             = @nombre,
  telefono           = @telefono,
  email              = @email
WHERE id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", p.Id);
            cmd.Parameters.AddWithValue("@dni", p.Dni);
            cmd.Parameters.AddWithValue("@apellido", p.Apellido);
            cmd.Parameters.AddWithValue("@nombre", p.Nombre);
            cmd.Parameters.AddWithValue("@telefono", (object?)p.Telefono ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@email", (object?)p.Email ?? DBNull.Value);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        /// <summary>
        /// Soft delete: setea fecha_eliminacion = NOW()
        /// </summary>
        public async Task<bool> DeleteAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"UPDATE propietarios 
                                 SET fecha_eliminacion=@fecha 
                                 WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fecha", DateTime.UtcNow);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        public async Task<bool> UpdateFechaEliminacionAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            String sql = "UPDATE propietarios SET fecha_eliminacion = @fecha_eliminacion WHERE id=@id";
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fecha_eliminacion", DBNull.Value);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }


        public async Task<(IEnumerable<Propietario> Items, int TotalCount)> GetPagedAsync(int page = 1, int pageSize = 10)
        {
            var list = new List<Propietario>();

            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            // SQL con paginado
            string sql = @"
                SELECT SQL_CALC_FOUND_ROWS *
                FROM propietarios 
                WHERE fecha_eliminacion IS NULL
                ORDER BY apellido, nombre
                LIMIT @offset, @pageSize";

            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
            cmd.Parameters.AddWithValue("@pageSize", pageSize);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(Map(reader));
            }

            // Obtener el total
            await reader.CloseAsync();
            var countCmd = new MySqlCommand("SELECT FOUND_ROWS()", conn);
            var total = Convert.ToInt32(await countCmd.ExecuteScalarAsync());

            return (list, total);
        }

        // =========================
        // Mapeo
        // =========================
        private static Propietario Map(MySqlDataReader r) => new()
        {
            Id                = r.GetInt64("id"),
            Dni               = r.GetString("dni"),
            Apellido          = r.GetString("apellido"),
            Nombre            = r.GetString("nombre"),
            Telefono          = r.IsDBNull(r.GetOrdinal("telefono")) ? null : r.GetString("telefono"),
            Email             = r.IsDBNull(r.GetOrdinal("email")) ? null : r.GetString("email"),
            FechaEliminacion  = r.IsDBNull(r.GetOrdinal("fecha_eliminacion")) ? null : r.GetDateTime("fecha_eliminacion")
        };
    }
}
