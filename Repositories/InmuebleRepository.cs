using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Inmobiliaria.Data;
using Inmobiliaria.Models;
using MySqlConnector;

namespace Inmobiliaria.Repositories
{
    public class InmuebleRepository : IInmuebleRepository
    {
        private readonly IMySqlConnectionFactory _connectionFactory;

        public InmuebleRepository(IMySqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }

        // =========================
        // Lecturas
        // =========================
        public async Task<IEnumerable<Inmueble>> GetAllAsync()
        {
            var list = new List<Inmueble>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            string sql = @"SELECT * FROM inmuebles";
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapInmueble(reader));

            return list;
        }
        public async Task<IEnumerable<Inmueble>> GetAllWithFiltersAsync(bool fecha_eliminacion = false)
        {
            var list = new List<Inmueble>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();
            string sql = @"SELECT * FROM inmuebles";
            if (fecha_eliminacion)
            {
                sql += " WHERE fecha_eliminacion IS NULL";
            }
            using var cmd = new MySqlCommand(sql, conn);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
                list.Add(MapInmueble(reader));

            return list;
        }

        public async Task<Inmueble?> GetByIdAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"SELECT * FROM inmuebles WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
                return MapInmueble(reader);

            return null;
        }

        public async Task<IEnumerable<Inmueble>> GetByPropietarioAsync(long propietarioId)
        {
            var list = new List<Inmueble>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"SELECT * FROM inmuebles 
                                 WHERE propietario_id=@propietario_id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@propietario_id", propietarioId);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapInmueble(reader));

            return list;
        }

        /// <summary>
        /// Disponibles = no suspendidos. (La disponibilidad por fechas se valida contra Contratos en otra consulta.)
        /// </summary>
        public async Task<IEnumerable<Inmueble>> GetDisponiblesAsync()
        {
            var list = new List<Inmueble>();
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"SELECT * FROM inmuebles WHERE suspendido = 0";
            using var cmd = new MySqlCommand(sql, conn);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                list.Add(MapInmueble(reader));

            return list;
        }

        // =========================
        // Escrituras
        // =========================
        public async Task<long> CreateAsync(Inmueble i)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
INSERT INTO inmuebles
(propietario_id, tipo_id, uso, ambientes, direccion, coordenada_lat, coordenada_lon, 
 precio_sugerido, suspendido, observaciones, created_at, updated_at)
VALUES
(@propietario_id, @tipo_id, @uso, @ambientes, @direccion, @coordenada_lat, @coordenada_lon,
 @precio_sugerido, @suspendido, @observaciones, @created_at, @updated_at);
SELECT LAST_INSERT_ID();";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@propietario_id", i.PropietarioId);
            cmd.Parameters.AddWithValue("@tipo_id", i.TipoId);
            cmd.Parameters.AddWithValue("@uso", i.Uso);
            cmd.Parameters.AddWithValue("@ambientes", i.AmbitosOrAmbientes()); // helper abajo mantiene compatibilidad de nombre
            cmd.Parameters.AddWithValue("@direccion", i.Direccion);
            cmd.Parameters.AddWithValue("@coordenada_lat", (object?)i.CoordenadaLat ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@coordenada_lon", (object?)i.CoordenadaLon ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@precio_sugerido", i.PrecioSugerido);
            cmd.Parameters.AddWithValue("@suspendido", i.Suspendido ? 1 : 0);
            cmd.Parameters.AddWithValue("@observaciones", (object?)i.Observaciones ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@created_at", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt64(result);
        }

        public async Task<bool> UpdateAsync(Inmueble i)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"
UPDATE inmuebles SET
  propietario_id = @propietario_id,
  tipo_id        = @tipo_id,
  uso            = @uso,
  ambientes      = @ambientes,
  direccion      = @direccion,
  coordenada_lat = @coordenada_lat,
  coordenada_lon = @coordenada_lon,
  precio_sugerido= @precio_sugerido,
  suspendido     = @suspendido,
  observaciones  = @observaciones,
  updated_at     = @updated_at
WHERE id = @id;";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", i.Id);
            cmd.Parameters.AddWithValue("@propietario_id", i.PropietarioId);
            cmd.Parameters.AddWithValue("@tipo_id", i.TipoId);
            cmd.Parameters.AddWithValue("@uso", i.Uso);
            cmd.Parameters.AddWithValue("@ambientes", i.AmbitosOrAmbientes());
            cmd.Parameters.AddWithValue("@direccion", i.Direccion);
            cmd.Parameters.AddWithValue("@coordenada_lat", (object?)i.CoordenadaLat ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@coordenada_lon", (object?)i.CoordenadaLon ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@precio_sugerido", i.PrecioSugerido);
            cmd.Parameters.AddWithValue("@suspendido", i.Suspendido ? 1 : 0);
            cmd.Parameters.AddWithValue("@observaciones", (object?)i.Observaciones ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        /// <summary>
        /// “Delete” lógico usando suspendido=1 (tu tabla inmuebles no tiene fecha_eliminacion).
        /// </summary>
        public async Task<bool> DeleteAsync(long id)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"UPDATE inmuebles 
                                 SET fecha_eliminacion=@fecha_eliminacion, updated_at=@updated_at 
                                 WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@fecha_eliminacion", DateTime.UtcNow);
            cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
            
        }

        public async Task<bool> UpdateSuspendidoAsync(long id, bool suspendido)
        {
            using var conn = _connectionFactory.CreateConnection();
            await conn.OpenAsync();

            const string sql = @"UPDATE inmuebles 
                                 SET suspendido=@suspendido, updated_at=@updated_at 
                                 WHERE id=@id";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@suspendido", suspendido ? 1 : 0);
            cmd.Parameters.AddWithValue("@updated_at", DateTime.UtcNow);

            var rows = await cmd.ExecuteNonQueryAsync();
            return rows > 0;
        }

        /// <summary>
        /// Compatibilidad con la interfaz: como la tabla no tiene fecha_eliminacion,
        /// mapeamos este método a “suspender” el inmueble.
        /// </summary>
        public Task<bool> UpdateFechaEliminacionAsync(long id)
        {
            return UpdateSuspendidoAsync(id, true);
        }

        // =========================
        // Mapeo
        // =========================
        private static Inmueble MapInmueble(MySqlDataReader r) => new()
        {
            Id             = r.GetInt32("id"),
            PropietarioId  = r.GetInt32("propietario_id"),
            TipoId         = r.GetInt32("tipo_id"),
            Uso            = r.GetString("uso"),
            Ambientes      = r.GetInt32("ambientes"),
            Direccion      = r.GetString("direccion"),
            CoordenadaLat  = r.IsDBNull(r.GetOrdinal("coordenada_lat")) ? null : r.GetDouble("coordenada_lat"),
            CoordenadaLon  = r.IsDBNull(r.GetOrdinal("coordenada_lon")) ? null : r.GetDouble("coordenada_lon"),
            PrecioSugerido = r.GetDecimal("precio_sugerido"),
            Suspendido     = r.GetBoolean("suspendido"),
            Observaciones  = r.IsDBNull(r.GetOrdinal("observaciones")) ? null : r.GetString("observaciones"),
            CreatedAt      = r.GetDateTime("created_at"),
            UpdatedAt      = r.GetDateTime("updated_at")
        };
    }

    // Pequeño helper para permitir mantener el nombre “Ambientes” en DB y en el modelo
    internal static class InmuebleExtensions
    {
        public static int AmbitosOrAmbientes(this Inmobiliaria.Models.Inmueble i) => i.Ambientes;
    }
}
